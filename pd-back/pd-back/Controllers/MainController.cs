using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotoDuel.Models.Web;
using PhotoDuel.Services;

namespace PhotoDuel.Controllers
{
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<MainController> _logger;

        private readonly JsonSerializerSettings _converterSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        public MainController(UserService userService, ILogger<MainController> logger)
        {
            _userService = userService;
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
            using var reader = new StreamReader(Request.Body);
            var body = reader.ReadToEnd();
            
            _logger.LogInformation($"REQUEST:\n{body}");
            var request = JsonConvert.DeserializeObject<InitRequest>(body, _converterSettings);

            var user = _userService.Init(request);
            var result = JsonConvert.SerializeObject(user, _converterSettings);
            
            _logger.LogInformation($"RESPONSE:\n{result}");

            Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            return Response.WriteAsync(result);
        }
    }
}