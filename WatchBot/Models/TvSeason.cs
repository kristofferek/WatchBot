using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models
{
    public class TvSeason
    {
        public int Id { get; set; }
        public int SeasonNumber { get; set; }
        public int NumberOfEpisodes { get; set; }
        public string FirstAirDate { get; set; }
        public string Poster { get; set; }
    }
}