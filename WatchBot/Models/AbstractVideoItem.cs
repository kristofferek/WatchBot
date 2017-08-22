using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models
{
    public class AbstractVideoItem
    {
        public AbstractVideoItem()
        {
            Genres = new Dictionary<string, int>();
        }

        public Dictionary<string, int> Genres { get; set; }
        public int Id { get; set; }
        public string Backdrop { get; set; }
        public string Poster { get; set; }
        public string Title { get; set; }
        public double Rating { get; set; }
        public string Description { get; set; }
        public string ProminentColor { get; set; }
        public ICollection<Actor> Actors { get; set; }

    }
}