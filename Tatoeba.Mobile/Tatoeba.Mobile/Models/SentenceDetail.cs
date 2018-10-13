using System.Collections.Generic;

namespace Tatoeba.Mobile.Models
{
    public class SentenceDetail
    {
        public string Id { get; set; }
        public List<Contribution> Sentences { get; set; } = new List<Contribution>();
        public List<Log> Logs { get; set; } = new List<Log>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public bool IsEditable { get; set; }
    }
}
