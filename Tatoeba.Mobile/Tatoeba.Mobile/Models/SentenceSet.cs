using System.Collections.Generic;
using System.Linq;

namespace Tatoeba.Mobile.Models
{
    public class SentenceSet : SentenceSetBase
    {
        public IEnumerable<Contribution> Translations => Sentences.Skip(1);
        public IEnumerable<Contribution> DirectTranslations => Sentences.Where(x => x.TranslationType == TranslationType.Direct);
        public IEnumerable<Contribution> IndirectTranslations => Sentences.Where(x => x.TranslationType == TranslationType.Indirect);

        override protected int MaxTranslationCount => 1;

        internal void Add(Contribution contribution)
        {
            Sentences.Add(contribution);
        }
    }
}
