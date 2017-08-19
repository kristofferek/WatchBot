using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class MovieInfoViewModel
    {
        public Movie Movie { get; set; }
        public CarouselViewModel Similar { get; set; }
    }
}