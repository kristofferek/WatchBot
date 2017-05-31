using System;
using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class GenreViewModel
    {
        public string GenreName { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public ICollection<MovieViewModel> Movies { get; set; }

    }
}