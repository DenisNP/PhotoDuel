using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoDuel.Models;
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
        private readonly ConcurrencyService _concurrencyService;
        private readonly ILogger<MainController> _logger;

        public MainController(
            UserService userService,
            DuelService duelService,
            ContentService contentService,
            ISocialService socialService,
            ConcurrencyService concurrencyService,
            ILogger<MainController> logger
        )
        {
            _userService = userService;
            _duelService = duelService;
            _contentService = contentService;
            _socialService = socialService;
            _concurrencyService = concurrencyService;
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
                user = _userService.CheckShuffles(user);
                var myDuels = _userService.LoadMyDuels(req.UserId);
                var winners = new Winner[0]; // TODO _userService.LoadPantheon().ToArray();

                // check if voting or load duel by link
                var additionalDuel = _duelService.LoadAdditional(req.UserId, req.Vote, req.DuelId, myDuels, out var message);
                var voted = _userService.TryVote(additionalDuel, user, req.Vote, out var voteMessage);

                // return response
                return new InitResponse
                {
                    User = user,
                    MyDuels = myDuels.OrderBy(d => d.Status).ThenByDescending(d => d.Creator.Time).ToArray(),
                    Pantheon = winners,
                    Categories = _contentService.GetCategories(),
                    Message = string.IsNullOrEmpty(voteMessage) ? message : voteMessage,
                    Voted = voted
                };
            });
        }

        [HttpPost("/create")]
        public Task Create()
        {
            return HandleRequest<CreateDuelRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.CreateDuel(req.UserId, req.Image.Trim(), req.PhotoId, req.ChallengeId)
                },
                true
            );
        }

        [HttpPost("/publish")]
        public Task Publish()
        {
            return HandleRequest<DuelIdRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.MakePublic(req.UserId, req.DuelId)
                }
            );
        }

        [HttpPost("/shuffle")]
        public Task Shuffle()
        {
            return HandleRequest<BaseRequest, UserResponse>(
                req => new UserResponse
                {
                    User = _userService.ShuffleChallenges(req.UserId)
                }
            );
        }

        [HttpPost("/join")]
        public Task Join()
        {
            return HandleRequest<JoinDuelRequest, DuelResponse>(
                req => new DuelResponse
                {
                    Duel = _duelService.JoinDuel(req.UserId, req.DuelId, req.Image.Trim(), req.PhotoId)
                },
                true
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
                },
                true
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
        
        private Task HandleRequest<TRequest, TResponse>(Func<TRequest, TResponse> handler, bool limitRatio = false) where TRequest : BaseRequest
        {
            // setup response
            Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            
            // load request body
            using var reader = new StreamReader(Request.Body);
            var body = reader.ReadToEnd();
            
            _logger.LogInformation($"REQUEST:\n{body}");
            var request = JsonConvert.DeserializeObject<TRequest>(body, Utils.ConverterSettings);

            // check sign
            if (request == null || !_socialService.IsSignValid(request.UserId, request.Params, request.Sign))
            {
                _logger.LogWarning("Signature calculation failed");
                Response.StatusCode = 400;
                return Response.WriteAsync(new ErrorResponse("Signature calculation failed").ToString());
            }
            
            // check requests ratio
            if (limitRatio)
            {
                if (!_concurrencyService.CheckAddRequest(request.UserId))
                {
                    _logger.LogWarning("Too many requests");
                    Response.StatusCode = 400;
                    return Response.WriteAsync(new ErrorResponse("Too many requests").ToString());
                }
            }
            
            // handle request
            try
            {
                var response = handler(request);
                var stringResponse = JsonConvert.SerializeObject(response, Utils.ConverterSettings);
            
                _logger.LogInformation($"RESPONSE:\n{stringResponse}");
                return Response.WriteAsync(stringResponse);
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                _logger.LogWarning(e.Message);
                Response.StatusCode = 400;
                return Response.WriteAsync(new ErrorResponse(e.Message).ToString());
            }
        }
    }
}