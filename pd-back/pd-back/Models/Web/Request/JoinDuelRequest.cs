﻿namespace PhotoDuel.Models.Web.Request
{
    public class JoinDuelRequest : DuelIdRequest
    {
        public string Image { get; set; }
        public string PhotoId { get; set; }
    }
}