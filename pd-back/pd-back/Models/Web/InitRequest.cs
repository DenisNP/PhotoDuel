namespace PhotoDuel.Models.Web
{
    public class InitRequest : BaseRequest
    {
        public string UserName { get; set; }
        public string UserPhoto { get; set; }
        public string[] FriendIds { get; set; }
    }
}