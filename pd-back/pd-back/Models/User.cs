namespace PhotoDuel.Models
{
    public class User : UserMeta
    {
        public int Rating { get; set; }
        public long LastShuffle { get; set; }
        public int[] ChallengeIds { get; set; }
        public int PublicDuelId { get; set; } = -1;
        
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