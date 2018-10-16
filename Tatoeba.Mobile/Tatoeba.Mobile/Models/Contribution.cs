using System.Collections.Generic;
using System.Linq;

namespace Tatoeba.Mobile.Models
{
    public class Contribution : SentenceBase
    {
        public string DateText { get; set; }
        public ContribType ContribType { get; set; }
        public TranslationType TranslationType { get; set; }
    }

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

    public class SearchResults
    {
        public SearchRequest Request { get; set; }
        public List<string> Pages { get; set; } = new List<string>();
        public List<SentenceSet> Results { get; set; } = new List<SentenceSet>();
    }

    public class SentenceSet : SentenceSetBase
    {
        public IEnumerable<Contribution> Translations => Sentences.Skip(1);
        public IEnumerable<Contribution> DirectTranslations => Sentences.Where(x=>x.TranslationType == TranslationType.Direct);
        public IEnumerable<Contribution> IndirectTranslations => Sentences.Where(x => x.TranslationType == TranslationType.Indirect);

        override protected int MaxTranslationCount => 5;

        internal void Add(Contribution contribution)
        {
            Sentences.Add(contribution);
        }
    }
}
