namespace PhotoDuel.Models.Web.Request
{
    public class CreateDuelRequest : BaseRequest
    {
        public string Image { get; set; }
        public string PhotoId { get; set; }
        public int ChallengeId { get; set; } = -1;
    }
}