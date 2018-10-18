using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using Xamarin.Forms;

namespace Tatoeba.Mobile.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public SearchResults searchResults;

        public SearchResultsViewModel(SearchResults searchResults)
        {
            this.Title = "Search: " + searchResults.Request.Text;
            this.searchResults = searchResults;
            Pages = Enumerable.Range(1, searchResults.PageCount).ToList();

            ToggleCommand = new Command(async (i) => await ToggleExpand((SentenceSet)i));
            ChangePageCommand = new Command<int>(async (i) => await ExecuteSearchCommand(i));
            PreviousPageCommand = new Command(async () => await ExecuteSearchCommand(searchResults.Request.Page - 1));
            NextPageCommand = new Command(async () => await ExecuteSearchCommand(searchResults.Request.Page + 1));

            PopulateList();
        }

        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; } 
            = new ObservableCollection<Grouping<string, object>>();

        public List<int> Pages { get; private set; }

        public int SelectedPage
        {
            get => this.searchResults.Request.Page;
            set
            {
                if (value == this.searchResults.Request.Page)
                {
                    return;
                }

                ExecuteSearchCommand(value);
            }
        }

        public bool EnablePagination => searchResults.PageCount > 0;
        public bool EnablePrevious => searchResults.Request.Page != Pages.FirstOrDefault();
        public bool EnableNext => searchResults.Request.Page != Pages.LastOrDefault();
        public string PreviousTextColor => EnablePrevious ? "#4CAF50" : "Gray";
        public string NextTextColor => EnableNext ? "#4CAF50" : "Gray";

        public Command PreviousPageCommand { get; set; }
        public Command NextPageCommand { get; set; }

        public Command ToggleCommand { get; set; }
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

        public Command ChangePageCommand { get; set; }
        public async Task ExecuteSearchCommand(int page)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            searchResults.Request.Page = page;

            var response = await MainService.SearchAsync(searchResults.Request);

            if (response.Status != TatoebaStatus.Success)
            {
                OnError(response.Status);
                IsBusy = false;
                return;
            }
            
            searchResults = response.Content;
            PopulateList();

            OnPropertyChanged(nameof(EnablePrevious));
            OnPropertyChanged(nameof(EnableNext));
            OnPropertyChanged(nameof(PreviousTextColor));
            OnPropertyChanged(nameof(NextTextColor));
            OnPropertyChanged(nameof(SelectedPage));

            IsBusy = false;      
        }

        private void PopulateList()
        {
            GroupedCells.Clear();

            foreach (var sentenceSet in searchResults.Results)
            {
                GroupedCells.Add(new Grouping<string, object>(sentenceSet.Original.Id, sentenceSet.CollapsableSentences));
            }            
        }

    }
}
