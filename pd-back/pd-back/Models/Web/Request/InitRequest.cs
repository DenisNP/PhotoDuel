using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoDuel.Models.Web.Request
{
    public class InitRequest : BaseRequest
    {
        public string UserName { get; set; }
        public string UserPhoto { get; set; }
        public string[] FriendIds { get; set; } = new string[0];
        
        public string DuelId { get; set; } = "";

        [JsonConverter(typeof(StringEnumConverter))]
        public Vote Vote { get; set; } = Vote.None;
    }

    public enum Vote
    {
        None,
        Creator,
        Opponent
    }
}