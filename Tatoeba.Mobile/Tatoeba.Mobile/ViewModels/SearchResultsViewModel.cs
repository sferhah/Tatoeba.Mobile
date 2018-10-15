using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Tatoeba.Mobile.Models;

namespace Tatoeba.Mobile.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        readonly List<SentenceSet> searchResults;

        public SearchResultsViewModel(List<SentenceSet> searchResults)
        {
            this.searchResults = searchResults;

            foreach(var sentenceSet in searchResults)
            {
                GroupedCells.Add(new Grouping<string, object>(sentenceSet.Original.Id, sentenceSet.Sentences));
            }
        }
        
        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();
    }
}
