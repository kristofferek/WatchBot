using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models.ViewModels
{
    public class TvShowInfoViewModel
    {
        public TvShow TvShow { get; set; }
        public CarouselViewModel Similar { get; set; }
    }
}