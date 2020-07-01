using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotoDuel.Models.Web.Request;
using PhotoDuel.Models.Web.Response;
using PhotoDuel.Services;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DuelService _duelService;
        private readonly ContentService _contentService;
        private readonly ISocialService _socialService;
        private readonly ILogger<MainController> _logger;

        private readonly JsonSerializerSettings _converterSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public MainController(
            UserService userService,
            DuelService duelService,
            ContentService contentService,
            ISocialService socialService,
            ILogger<MainController> logger
        )
        {
            _userService = userService;
            _duelService = duelService;
            _contentService = contentService;
            _socialService = socialService;
            _logger = logger;
        }
        
        [HttpGet("/test")]
        public ContentResult Test()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                Content = "<h1>It works!</h1>"
            };
        }

        [HttpPost("/init")]
        public Task Init()
        {
            return HandleRequest<InitRequest, InitResponse>(req =>
            {
                var user = _userService.LoadUser(req.UserId);
                var publicDuels = _userService.LoadPublicDuels().ToArray();
                var friendDuels = _userService.LoadFriendDuels(req.FriendIds).ToArray();
                var (myDuels, current) = _userService.LoadMyDuels(req.UserId);
                var winners = _userService.LoadPantheon().ToArray();

                // check if voting or load duel by link
                var duelVoted = _userService.PreloadAndVote(user, req.DuelId, req.Vote, ref current, out var message);
                if (duelVoted != null) myDuels.Add(duelVoted);

                return new InitResponse
                {
                    User = user,
                    Duel = current,
                    PublicDuels = publicDuels,
                    FriendDuels = friendDuels,
                    MyDuels = myDuels.ToArray(),
                    Pantheon = winners,
                    Categories = _contentService.GetCategories(),
                    Message = message
                };
            });
        }

        [HttpPost("/create")]
        public Task Create()
        {
            return HandleRequest<CreateDuelRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.CreateDuel(req.UserId, req.Image, req.Type, req.ChallengeId)
                }
            );
        }

        [HttpPost("/join")]
        public Task Join()
        {
            return HandleRequest<JoinDuelRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.JoinDuel(req.UserId, req.DuelId, req.Image)
                }
            );
        }
        
        [HttpPost("/delete")]
        public Task Delete()
        {
            return HandleRequest<DuelIdRequest, OkResponse>(
                req => new OkResponse
                {
                    Ok = _duelService.DeleteDuel(req.UserId, req.DuelId)
                }
            );
        }
        
        [HttpPost("/report")]
        public Task Report()
        {
            return HandleRequest<DuelIdRequest, OkResponse>(
                req => new OkResponse
                {
                    Ok = _duelService.ReportDuel(req.UserId, req.DuelId)
                }
            );
        }
        
        [HttpPost("/updateStory")]
        public Task UpdateStory()
        {
            return HandleRequest<UpdateStoryRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.UpdateStory(req.UserId, req.DuelId, req.StoryUrl)
                }
            );
        }
        
        private Task HandleRequest<TRequest, TResponse>(Func<TRequest, TResponse> handler) where TRequest : BaseRequest
        {
            // setup response
            Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            // load request body
            using var reader = new StreamReader(Request.Body);
            var body = reader.ReadToEnd();
            
            _logger.LogInformation($"REQUEST:\n{body}");
            var request = JsonConvert.DeserializeObject<TRequest>(body, _converterSettings);

            // check sign
            if (request == null || !_socialService.IsSignValid(request.UserId, request.Params, request.Sign))
            {
                _logger.LogWarning("Signature calculation failed");
                Response.StatusCode = 400;
                return Response.WriteAsync("{}");
            }
            
            // handle request
            var response = handler(request);
            var stringResponse = JsonConvert.SerializeObject(response, _converterSettings);
            
            _logger.LogInformation($"RESPONSE:\n{stringResponse}");
            return Response.WriteAsync(stringResponse);
        }
    }
}