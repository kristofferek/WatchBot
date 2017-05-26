﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using WatchBot.Models;

namespace WatchBot.Models
{
    public class MovieViewModel
    {

        public MovieViewModel()
        {
            this.Genres = new List<Genre>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Backdrop { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public string ImdbID { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public string Poster { get; set; }
        public int Runtime { get; set; }
        public string ReleaseDate { get; set; }
        public string ProminentColor { get; set; }
        public ICollection<ActorViewModel> Actors { get; set; }

        public int GetHours()
        {
            return (Runtime / 60);
        }

        public int GetMinutes()
        {
            return (Runtime % 60);
        }
    }
}