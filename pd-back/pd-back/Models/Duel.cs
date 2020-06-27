using System.Collections.Generic;

namespace PhotoDuel.Models
{
    public class Duel : IIdentity
    {
        public string Id { get; set; }
        public Duellist Creator { get; set; }
        public Duellist Opponent { get; set; }
        public long TimeStart { get; set; }
        public long TimeFinish { get; set; }
        public DuelStatus Status { get; set; }
        public int Category { get; set; }
        public string Challenge { get; set; }
    }
    
    public class Duellist
    {
        public UserMeta User { get; set; }
        public string Image { get; set; }
        public List<UserMeta> Voters { get; set; }
    }

    public enum DuelStatus
    {
        Created,
        Started,
        Finished
    }
}