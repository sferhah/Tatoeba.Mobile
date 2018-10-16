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
        readonly List<SentenceSet> searchResults;

        public SearchResultsViewModel(List<SentenceSet> searchResults)
        {
            this.searchResults = searchResults;

            ToggleCommand = new Command(async (i) => await ToggleExpand((SentenceSet)i));

            foreach (var sentenceSet in searchResults)
            {
                GroupedCells.Add(new Grouping<string, object>(sentenceSet.Original.Id, sentenceSet.CollapsableTranslations));
            }
        }

        public Command ToggleCommand { get; set; }
        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } = new ObservableCollection<Grouping<string, object>>();

        public async Task ToggleExpand(SentenceSet sentenceSet)
        {
            sentenceSet.IsExpanded = !sentenceSet.IsExpanded;

            await Task.Delay(1); // Hack, otherwise Uwp crashes. 

            int index = searchResults.IndexOf(sentenceSet);

            var group = GroupedCells[index];
            group.Clear();

            foreach (var item in sentenceSet.CollapsableTranslations)
            {
                group.Add(item);
            }
        }

    }
}
