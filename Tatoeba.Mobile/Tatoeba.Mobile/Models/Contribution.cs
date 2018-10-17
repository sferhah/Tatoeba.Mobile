namespace Tatoeba.Mobile.Models
{
    public class Contribution : SentenceBase
    {
        public string DateText { get; set; }
        public ContribType ContribType { get; set; }
        public TranslationType TranslationType { get; set; }
    }
}
