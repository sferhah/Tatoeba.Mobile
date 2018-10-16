using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        readonly SearchResults searchResults;

        public SearchResultsViewModel(SearchResults searchResults)
        {
            this.Title = "Search: " + searchResults.Query;
            this.searchResults = searchResults;

            ToggleCommand = new Command(async (i) => await ToggleExpand((SentenceSet)i));

            foreach (var sentenceSet in searchResults.Results)
            {
                GroupedCells.Add(new Grouping<string, object>(sentenceSet.Original.Id, sentenceSet.CollapsableSentences));
            }
        }

        public Command ToggleCommand { get; set; }
        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();

        public List<string> Pages => searchResults.Pages;

        public async Task ToggleExpand(SentenceSet sentenceSet)
        {
            sentenceSet.IsExpanded = !sentenceSet.IsExpanded;

            await Task.Delay(1); // Hack, otherwise Uwp crashes. 

            int index = searchResults.Results.IndexOf(sentenceSet);

            var group = GroupedCells[index];
            group.Clear();

            foreach (var item in sentenceSet.CollapsableSentences)
            {
                group.Add(item);
            }
        }

    }
}
