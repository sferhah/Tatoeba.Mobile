namespace Tatoeba.Mobile.Models
{
    public class SearchRequest
    {
        public int Page { get; set; }
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
