namespace PhotoDuel.Models.Web.Request
{
    public class CreateDuelRequest : BaseRequest
    {
        public string Image { get; set; }
        public int ChallengeId { get; set; } = -1;
        public string ChallengeText { get; set; } = "";
    }
}