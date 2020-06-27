namespace PhotoDuel.Models
{
    public class User : UserMeta
    {
        public int Rating { get; set; }
    }
    
    public class UserMeta : IIdentity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Photo { get; set; }
    }
}