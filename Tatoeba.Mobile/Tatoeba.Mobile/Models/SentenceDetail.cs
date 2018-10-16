using System.Collections.Generic;
using System.Linq;

namespace Tatoeba.Mobile.Models
{
    public class ExpandToggler
    {
        public SentenceSetBase SentenceSetBase { get ; private set; }
        public ExpandToggler(SentenceSetBase sentenceSetBase)
        {
            this.SentenceSetBase = sentenceSetBase;
        }

        public string Text =>
            SentenceSetBase.IsExpanded ?
            $"▲ Fewer translations"
            : $"▼ Show {SentenceSetBase.AllTranslations.Count() - SentenceSetBase.MinTranslations.Count()} more translations";

        public string BackgroundColor =>            
            SentenceSetBase.IsExpanded ?
            "#0066AA"
            : "#DDE7EE";

        public string TextColor =>
            SentenceSetBase.IsExpanded ?
            "White"
            : "#0066AA";
    }

    public class SentenceSetBase
    {
        public string Id { get; set; }
        public List<Contribution> Sentences { get; set; } = new List<Contribution>();
        public Contribution Original => Sentences.FirstOrDefault();
        public IEnumerable<Contribution> AllTranslations => Sentences.Skip(1);  

        virtual protected int MaxTranslationCount => 1;

        public IEnumerable<Contribution> MinTranslations => Sentences.Skip(1).Take(MaxTranslationCount);

        public bool IsExpanded { get; set; }

        public bool EnableShowMore => (AllTranslations.Count() - MinTranslations.Count()) > 0;

        public IEnumerable<object> CollapsableTranslations => IsExpanded ? ExpandedTranslations : CollapsedTranslations;

        public IEnumerable<object> CollapsedTranslations
            => EnableShowMore ? MinTranslations.Concat(new List<object> { new ExpandToggler(this) }) : MinTranslations;

        public IEnumerable<object> ExpandedTranslations
            => EnableShowMore ? AllTranslations.Concat(new List<object> { new ExpandToggler(this) }) : MinTranslations;

        public string CountLabel =>
           EnableShowMore ? (IsExpanded ?
            $"{AllTranslations.Count()}"
            : $"{MinTranslations.Count()}/{AllTranslations.Count()}")
            : $"{AllTranslations.Count()}";
    }

    public class SentenceDetail : SentenceSetBase
    {
        public List<Log> Logs { get; set; } = new List<Log>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public bool IsEditable { get; set; }
        override protected int MaxTranslationCount => 6;
    }
}
