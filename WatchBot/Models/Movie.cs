using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using WatchBot.Models;

namespace WatchBot.Models
{
    public class Movie : AbstractVideoItem
    {

        public string Trailer { get; set; }
        public int Runtime { get; set; }
        public string ReleaseDate { get; set; }

        public int GetRuntimeHours()
        {
            return (Runtime / 60);
        }

        public int GetRuntimeMinutes()
        {
            return (Runtime % 60);
        }
    }
}