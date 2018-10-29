using System.Collections.Generic;
using System.Linq;

namespace Tatoeba.Mobile.Models
{
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

        public IEnumerable<object> CollapsableSentences => IsExpanded ?
            new List<object> { Original }.Concat(ExpandedTranslations)
            : new List<object> { Original }.Concat(CollapsedTranslations);

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

    public class ExpandToggler
    {
        public SentenceSetBase SentenceSetBase { get; private set; }
        public ExpandToggler(SentenceSetBase sentenceSetBase)
        {
            this.SentenceSetBase = sentenceSetBase;
        }

        public string Text =>
            SentenceSetBase.IsExpanded ?
            $"▲ {Resx.AppResources.FewerTranslations}"
            : $"▼ {string.Format(Resx.AppResources.ShowMoreTranslations, SentenceSetBase.AllTranslations.Count() - SentenceSetBase.MinTranslations.Count())}";

        public string BackgroundColor =>
            SentenceSetBase.IsExpanded ?
            "#0066AA"
            : "#DDE7EE";

        public string TextColor =>
            SentenceSetBase.IsExpanded ?
            "White"
            : "#0066AA";
    }
}
