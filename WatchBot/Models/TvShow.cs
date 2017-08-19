using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models
{
    public class TvShow : AbstractVideoItem
    {
        public TvShow()
        {
            Seasons = new Dictionary<int, TvSeason>();
        }

        public int EpisodeRuntime { get; set; }
        public string FirstAirDate { get; set; }
        public string Network { get; set; }
        public Dictionary<int, TvSeason> Seasons { get; set; }
    }
}