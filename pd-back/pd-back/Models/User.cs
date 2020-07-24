namespace PhotoDuel.Models
{
    public class User : UserMeta
    {
        public long LastShuffle { get; set; }
        public int ShufflesLeft { get; set; }
        public int[] ChallengeIds { get; set; }
        public bool IsBanned { get; set; } = false;
        public Duel PublicDuel { get; set; } = null;
        
        public UserMeta ToMeta()
        {
            return new UserMeta
            {
                Id = Id,
                Name = Name,
                Photo = Photo
            };
        }
    }
    
    public class UserMeta : IIdentity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
    }
}