using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotoDuel.Models.Web;
using PhotoDuel.Models.Web.Request;
using PhotoDuel.Models.Web.Response;
using PhotoDuel.Services;

namespace PhotoDuel.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly DuelService _duelService;
        private readonly ILogger<MainController> _logger;

        private readonly JsonSerializerSettings _converterSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public MainController(UserService userService, DuelService duelService, ILogger<MainController> logger)
        {
            _userService = userService;
            _duelService = duelService;
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
            return HandleRequest<InitRequest, InitResponse>(_userService.Init);
        }

        [HttpPost("/create")]
        public Task Create()
        {
            return HandleRequest<CreateDuelRequest, DuelResponse>(_duelService.CreateDuel);
        }

        [HttpPost("/join")]
        public Task Join()
        {
            return HandleRequest<JoinDuelRequest, DuelResponse>(_duelService.JoinDuel);
        }
        
        [HttpPost("/delete")]
        public Task Delete()
        {
            return HandleRequest<DuelIdRequest, OkResponse>(
                request => new OkResponse
                {
                    Ok = _duelService.DeleteDuel(request.UserId, request.DuelId)
                }
            );
        }
        
        [HttpPost("/report")]
        public Task Report()
        {
            return HandleRequest<DuelIdRequest, OkResponse>(
                request => new OkResponse
                {
                    Ok = _duelService.ReportDuel(request.UserId, request.DuelId)
                }
            );
        }
        
        private Task HandleRequest<TRequest, TResponse>(Func<TRequest, TResponse> handler) where TRequest : BaseRequest
        {
            using var reader = new StreamReader(Request.Body);
            var body = reader.ReadToEnd();
            
            _logger.LogInformation($"REQUEST:\n{body}");
            var request = JsonConvert.DeserializeObject<TRequest>(body, _converterSettings);

            var response = handler(request);
            var stringResponse = JsonConvert.SerializeObject(response, _converterSettings);
            
            _logger.LogInformation($"RESPONSE:\n{stringResponse}");

            Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            return Response.WriteAsync(stringResponse);
        }
    }
}