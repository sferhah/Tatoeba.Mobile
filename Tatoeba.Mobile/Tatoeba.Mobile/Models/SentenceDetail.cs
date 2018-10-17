using System.Collections.Generic;

namespace Tatoeba.Mobile.Models
{
    public class SentenceDetail : SentenceSetBase
    {
        public List<Log> Logs { get; set; } = new List<Log>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public bool IsEditable { get; set; }
        override protected int MaxTranslationCount => 6;
    }
}
