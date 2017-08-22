using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WatchBot.Models.ViewModels
{
    public class CarouselViewModel
    {
        public CarouselViewModel()
        {
            Items = new Dictionary<int, PreviewItem>();
        }

        public string CarouselName { get; set; }
        public PreviewItem Highlight { get; set; }
        public Dictionary<int, PreviewItem> Items { get; set; }

    }
}