﻿namespace MVC2.Models
{
    public class Exchange
    {
        public DateTime date { get; set; }
        public string iso3 { get; set; }
        public string name { get; set; }
        public string buy { get; set; }
        public string sell { get; set; }
        public int unit { get; set; }
    }
}
