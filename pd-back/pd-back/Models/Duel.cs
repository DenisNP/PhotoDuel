using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoDuel.Models
{
    public class Duel : IIdentity
    {
        public string Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public DuelStatus Status { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public DuelType Type { get; set; }
        
        public Duellist Creator { get; set; }
        public Duellist Opponent { get; set; }

        public long TimeStart { get; set; }
        public long TimeFinish { get; set; }

        public int ChallengeId { get; set; } = -1;
        public string ChallengeText { get; set; } = "";
    }
    
    public class Duellist
    {
        public UserMeta User { get; set; }
        public string Image { get; set; }
        public List<UserMeta> Voters { get; set; }
        public long Time { get; set; }
    }

    public class Winner
    {
        public UserMeta User { get; set; }
        public string Image { get; set; }
        public int ChallengeId { get; set; }

        public Winner() { }

        public Winner(Duellist duellist, int challengeId)
        {
            User = duellist.User;
            Image = duellist.Image;
            ChallengeId = challengeId;
        }
    }

    public enum DuelStatus
    {
        Created,
        Started,
        Finished
    }

    public enum DuelType
    {
        Public,
        Friends,
        Private
    }
}