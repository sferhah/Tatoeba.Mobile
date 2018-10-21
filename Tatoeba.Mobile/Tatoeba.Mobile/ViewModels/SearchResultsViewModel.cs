using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;
using Tatoeba.Mobile.Storage;
using Xamarin.Forms;

namespace Tatoeba.Mobile.ViewModels
{
    public class SearchResultsViewModel : BaseViewModel
    {
        public SearchResults searchResults;

        public SearchResultsViewModel()
        {
            this.Title = "Browse by language";

            SelectedLanguage = MainService.BrowsableLanguages.Where(x => x.Iso == (LocalSettings.LastBrowsedIso ?? "eng")).FirstOrDefault();
            SelectedTransLanguage = MainService.TransBrowsableLanguages.Where(x => x.Iso == (LocalSettings.LastTransBrowsedIso ?? "none")).FirstOrDefault();

            this.searchResults = new SearchResults();
            this.searchResults.Request.Mode = QueryMode.Browse;
            searchResults.Request.IsoFrom = LocalSettings.LastBrowsedIso = SelectedLanguage.Iso;
            searchResults.Request.IsoTo = LocalSettings.LastTransBrowsedIso = SelectedTransLanguage.Iso;
            searchResults.Request.Page = 1;

            Init();

        }

        public SearchResultsViewModel(SearchResults searchResults)
        {
            this.Title = "Search: " + searchResults.Request.Text;
            this.searchResults = searchResults;
            SelectedLanguage = MainService.Languages.Where(x => x.Iso == LocalSettings.LastIsoSearchFrom).FirstOrDefault();

            Init();
        }

        private void Init()
        {
            ToggleCommand = new Command(async (i) => await ToggleExpand((SentenceSet)i));
            ChangePageCommand = new Command<int>(async (i) => await ExecuteSearchCommand(i));
            PreviousPageCommand = new Command(async () => await ExecuteSearchCommand(searchResults.Request.Page - 1));
            NextPageCommand = new Command(async () => await ExecuteSearchCommand(searchResults.Request.Page + 1));

            PopulateList();
        }

        protected override void OnFirstAppear()
        {
            base.OnFirstAppear();

            if (EnableBrowsing && GroupedCells.Count == 0)
            {
                ExecuteSearchCommand(1);
            }
        }

        public ObservableCollection<Grouping<string, object>> GroupedCells { get; private set; }
            = new ObservableCollection<Grouping<string, object>>();


        public Language SelectedLanguage { get; set; }
        public IEnumerable<string> LanguageList => MainService.BrowsableLanguages?.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string LanguageChoiceSource
        {
            get => SelectedLanguage?.Label;
            set
            {
                if (IsBusy || value == LanguageChoiceSource)
                {
                    return;
                }

                SelectedLanguage = MainService.BrowsableLanguages.FirstOrDefault(c => c.Label == value);
                searchResults.Request.IsoFrom = LocalSettings.LastBrowsedIso = SelectedLanguage.Iso;
                searchResults.Request.Page = 1;

                ExecuteSearchCommand(searchResults.Request.Page);
            }
        }



        public Language SelectedTransLanguage { get; set; }
        public IEnumerable<string> TransLanguageList => MainService.TransBrowsableLanguages?.Select(c => c.Label).ToList(); // To List needed by xamarin forms picker
        public string TransLanguageChoiceSource
        {
            get => SelectedTransLanguage?.Label;
            set
            {
                if (IsBusy || value == TransLanguageChoiceSource)
                {
                    return;
                }

                SelectedTransLanguage = MainService.TransBrowsableLanguages.FirstOrDefault(c => c.Label == value);
                searchResults.Request.IsoTo = LocalSettings.LastTransBrowsedIso = SelectedTransLanguage.Iso;

                ExecuteSearchCommand(searchResults.Request.Page);
            }
        }


        public List<int> Pages => Enumerable.Range(1, searchResults.PageCount).ToList();

        public int SelectedPage
        {
            get => this.searchResults.Request.Page;
            set
            {
                if (value == 0 || value == SelectedPage)
                {
                    return;
                }

                ExecuteSearchCommand(value);
            }
        }

        public bool EnableBrowsing => this.searchResults.Request.Mode == QueryMode.Browse;
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

            var response = await MainService.GetSentencesAsync(searchResults.Request);

            if (response.Status != TatoebaStatus.Success)
            {
                OnError(response);
                IsBusy = false;
                return;
            }
            
            searchResults = response.Content;            

            OnPropertyChanged(nameof(EnablePagination));
            OnPropertyChanged(nameof(EnablePrevious));
            OnPropertyChanged(nameof(EnableNext));
            OnPropertyChanged(nameof(PreviousTextColor));
            OnPropertyChanged(nameof(NextTextColor));
            OnPropertyChanged(nameof(Pages));
            OnPropertyChanged(nameof(SelectedPage));

            IsBusy = false;

            PopulateList();
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
