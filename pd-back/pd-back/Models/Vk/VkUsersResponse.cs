using Newtonsoft.Json;

namespace PhotoDuel.Models.Vk
{
    public class VkUsersResponse
    {
        [JsonProperty("response")]
        public VkUser[] Response { get; set; }
    }
}