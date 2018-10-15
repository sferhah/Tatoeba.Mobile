using System;
using Tatoeba.Mobile.ViewModels;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : TatoebaContentPage<SearchViewModel>
    {
        public SearchPage()
        {
            InitializeComponent();
            ViewModel = new SearchViewModel();
            search_entry.Completed += (s, e) => Button_Clicked(null, null);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(await ViewModel.ExecuteSearchCommand())
            {
                await Navigation.PushAsync(new SearchResultsPage(ViewModel.SearchResults));             
            }
        }
    }
}