namespace PhotoDuel.Models.Web
{
    public class InitResponse
    {
        public User User { get; set; }
        public Duel Duel { get; set; }
        public Duel[] PublicDuels { get; set; }
        public Duel[] FriendDuels { get; set; }
        public Duel[] MyDuels { get; set; }
        public Winner[] Pantheon { get; set; }
        public Category[] Categories { get; set; }
        public string Message { get; set; }
    }
}