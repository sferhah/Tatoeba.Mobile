namespace Tatoeba.Mobile.Models
{
    public enum QueryMode
    {
        Search,
        Browse,
    }

    public class SearchRequest
    {
        public QueryMode Mode { get; set; }
        public int Page { get; set; } = 1;
        public string Text { get; set; }
        public string IsoFrom { get; set; }
        public string IsoTo { get; set; }
        public bool? IsOrphan { get; set; }
        public bool? IsUnapproved { get; set; }
        public bool? HasAudio { get; set; }
        public bool? IsTransOrphan { get; set; }
        public bool? IsTransUnapproved { get; set; }
        public bool? TransHasAudio { get; set; }
    }
}
