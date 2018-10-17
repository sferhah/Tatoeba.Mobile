using System.Collections.Generic;

namespace Tatoeba.Mobile.Models
{
    public class SearchResults
    {
        public SearchRequest Request { get; set; }
        public int PageCount { get; set; }
        public List<SentenceSet> Results { get; set; } = new List<SentenceSet>();
    }
}
