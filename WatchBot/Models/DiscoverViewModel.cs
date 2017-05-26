using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WatchBot.Models;

namespace WatchBot.Models
{
    public class DiscoverViewModel
    {
        public MovieViewModel FeaturePopular { get; set; }
        public MovieViewModel FeatureTopRated { get; set; }
        public ICollection<MovieViewModel> Popular { get; set; }
        public ICollection<MovieViewModel> TopRated { get; set; }


    }
}