using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class DetailsViewModel
    {
        public MovieViewModel Movie { get; set; }
        public Dictionary<int, MovieViewModel> Similar { get; set; }
    }
}