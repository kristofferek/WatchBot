using System;
using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class GenreViewModel
    {
        public GenreViewModel()
        {
            Movies = new Dictionary<int, MovieViewModel>();
        }

        public string GenreName { get; set; }
        public int GenreId { get; set; }
        public Dictionary<int, MovieViewModel> Movies { get; set; }

    }
}