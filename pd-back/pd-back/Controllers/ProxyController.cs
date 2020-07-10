using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PhotoDuel.Controllers
{
    [ApiController]
    [Route("/proxy")]
    public class ProxyController : ControllerBase
    {
        [HttpPost]
        [Consumes("multipart/form-data")]
        public ContentResult Post([FromForm]FormData formData) {
            var server = Request.Query["server"];
            var client = new HttpClient();

            client.DefaultRequestHeaders.Accept.Clear();

            var multiContent = new MultipartFormDataContent();

            var file = formData.Photo;
            if (file != null)
            {
                var fileStreamContent = new StreamContent(file.OpenReadStream());
                multiContent.Add(fileStreamContent, "photo", file.FileName);
            }

            var response = client.PostAsync(server, multiContent).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;
            var res = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            return new ContentResult
            {
                Content = res,
                ContentType = "application/json",
            };
        } 
    }
    
    public class FormData {
        public IFormFile Photo { get; set; }
    }
}