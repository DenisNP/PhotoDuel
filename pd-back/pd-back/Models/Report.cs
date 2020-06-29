namespace PhotoDuel.Models
{
    public class Report : IIdentity
    {
        public string Id { get; set; }
        public UserMeta Reporter { get; set; }
        public Duel Duel { get; set; }
        public long Time { get; set; }
    }
}