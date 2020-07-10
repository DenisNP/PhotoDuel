using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PhotoDuel.Models
{
    public class Duel : IIdentity
    {
        public string Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public DuelStatus Status { get; set; }

        public bool IsPublic { get; set; }
        
        public Duellist Creator { get; set; }
        public Duellist Opponent { get; set; }

        public long TimeStart { get; set; }
        public long TimeFinish { get; set; }

        public int ChallengeId { get; set; } = -1;
        // public string ChallengeText { get; set; } = "";

        public static Expression<Func<Duel, bool>> IsCurrentDuelOf(string userId)
        {
            return d => d.Status != DuelStatus.Finished
                        && (
                            d.Creator.User.Id == userId
                            || d.Opponent != null && d.Opponent.User.Id == userId
                        );
        }
    }
    
    public class Duellist
    {
        public UserMeta User { get; set; }
        public string Image { get; set; } = "";
        public string PhotoId { get; set; } = "";
        public string Story { get; set; } = "";
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
}