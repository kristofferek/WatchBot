using System.Collections.Generic;

namespace WatchBot.Models.ViewModels
{
    public class SearchResultViewModel
    {
        public SearchResultViewModel()
        {
            Items = new Dictionary<int, PreviewItem>();
        }

        public string SearchString { get; set; }
        public Dictionary<int, PreviewItem> Items { get; set; }
    }
}