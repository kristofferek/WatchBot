using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models.ViewModels
{
    public class TVShowsViewModel
    {
        public TVShowsViewModel()
        {
            Carousels = new List<CarouselViewModel>();
        }

        public List<CarouselViewModel> Carousels { get; set; }
    }
}