namespace PhotoDuel.Models.Web.Request
{
    public class CreateDuelRequest : JoinDuelRequest
    {
        public int ChallengeId { get; set; } = -1;
    }
}