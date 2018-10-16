using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using System.Collections.Generic;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage(List<SentenceSet> searchResults)
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
        }

        async void AddItem_Clicked()
        {

        }
    }   
}