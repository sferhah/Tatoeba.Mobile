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
