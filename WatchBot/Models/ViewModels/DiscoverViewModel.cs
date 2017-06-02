using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WatchBot.Models;

namespace WatchBot.Models.ViewModels
{
    public class DiscoverViewModel
    {
        public MovieViewModel FeaturePopular { get; set; }
        public MovieViewModel FeatureTopRated { get; set; }
        public Dictionary<int, MovieViewModel> Popular { get; set; }
        public Dictionary<int, MovieViewModel> TopRated { get; set; }


    }
}