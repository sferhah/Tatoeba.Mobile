using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using System;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage()
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel();
            ToolbarItems.Add(new ToolbarItem
            {
                Text = Resx.AppResources.Browse,
                Icon = "add.png",
                Command = new Command(() => AddItem_Clicked()),
            });
        }

        public SearchResultsPage(SearchResults searchResults)
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel(searchResults);
        }

        async void AddItem_Clicked()
        {
            var target = new NewSentencePage(ViewModel.SelectedLanguage.Iso);

            target.ViewModel.Save += (s,e) => ViewModel.ExecuteSearchCommand(1); 

            await Navigation.PushModalAsync(new NavigationPage(target));
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