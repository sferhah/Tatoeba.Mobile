using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage(SearchResults searchResults)
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel(searchResults);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if ((args.SelectedItem is ExpandToggler toggler))
            {
                ItemsListView.SelectedItem = null;
                ViewModel.ToggleCommand.Execute(toggler.SentenceSetBase);
                return;
            }

            if ((args.SelectedItem is Contribution item))
            {
                await Navigation.PushAsync(new SentenceDetailPage(item.Id));
                ItemsListView.SelectedItem = null;
                return;
            }
        }
    }
}