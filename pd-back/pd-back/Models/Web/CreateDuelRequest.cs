using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoDuel.Models.Web
{
    public class CreateDuelRequest : BaseRequest
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DuelType Type { get; set; }
        public string Image { get; set; }
        public int ChallengeId { get; set; } = -1;
        public string ChallengeText { get; set; } = "";
    }
}