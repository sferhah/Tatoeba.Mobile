namespace Tatoeba.Mobile.Models
{
    public class SentenceBase
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public Direction Direction { get; set; }
        public Language Language { get; set; }
    }
}
