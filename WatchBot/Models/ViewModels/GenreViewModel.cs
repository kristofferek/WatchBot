using System;
using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class GenreViewModel
    {
        public GenreViewModel()
        {
            Movies = new Dictionary<int, PreviewItem>();
        }

        public string GenreName { get; set; }
        public int GenreId { get; set; }
        public Dictionary<int, PreviewItem> Movies { get; set; }

    }
}