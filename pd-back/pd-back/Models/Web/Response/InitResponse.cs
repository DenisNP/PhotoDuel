﻿namespace PhotoDuel.Models.Web.Response
{
    public class InitResponse : UserResponse
    {
        public Duel[] MyDuels { get; set; }
        public Winner[] Pantheon { get; set; }
        public Category[] Categories { get; set; }
        public string Message { get; set; }
        public bool Voted { get; set; }
    }
}