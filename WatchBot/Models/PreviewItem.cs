using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models
{
    public class PreviewItem
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Backdrop { get; set; }
        public string BackdropHighRes { get; set; }
        public string Poster { get; set; }
        public string Description { get; set; }
        public string ReleaseDate { get; set; }
        public int VoteCount { get; set; }
        public bool IsAMovie { get; set; }

    }
}