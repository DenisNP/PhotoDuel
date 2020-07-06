﻿using System.Collections.Generic;

namespace PhotoDuel.Models.Web.Request
{
    public class BaseRequest
    {
        public string UserId { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public string Sign { get; set; } = "";
    }
}