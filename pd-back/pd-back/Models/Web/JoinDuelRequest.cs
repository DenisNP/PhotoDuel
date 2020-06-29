namespace PhotoDuel.Models.Web
{
    public class JoinDuelRequest : BaseRequest
    {
        public string DuelId { get; set; }
        public string Image { get; set; }
    }
}