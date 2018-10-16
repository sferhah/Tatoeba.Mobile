using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage(SearchResults searchResults)
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel(searchResults);
           // BuildPageButtons(searchResults);
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

        void BuildPageButtons(SearchResults searchResults)
        {
            if (!searchResults.Pages.Any())
            {
                return;
            }

            bool enable_previous = searchResults.CurrentPage.ToString() != searchResults.Pages.First();
            var previous = new Button
            {
                Text = "◀",
                TextColor = enable_previous ? Color.FromHex("#4CAF50") : Color.Default,
                BackgroundColor = Color.Transparent,
            };

            if (enable_previous)
            {
                previous.Clicked += async (s, e) =>
                {
                    searchResults.CurrentPage--;
                    await Navigation.PushAsync(new SearchResultsPage(searchResults));
                };
            }

            pages_container.Children.Add(previous);

            foreach (var page in searchResults.Pages)
            {
                if (page == "...")
                {
                    var button = new Button
                    {
                        Text = page,
                        BackgroundColor = Color.Transparent,
                        TextColor = Color.Default,
                    };
                    pages_container.Children.Add(button);
                }
                else
                {
                    bool enable_button = page != searchResults.CurrentPage.ToString();

                    var button = new Button
                    {
                        Text = page,
                        BackgroundColor = enable_button ? Color.Default : Color.FromHex("#4CAF50"),
                        TextColor = enable_button ? Color.Default : Color.White,
                    };

                    if (enable_button)
                        button.Clicked += async (s, e) => await Navigation.PushAsync(new SearchResultsPage(searchResults));

                    pages_container.Children.Add(button);
                }
            }

            bool enable_next = searchResults.CurrentPage.ToString() != searchResults.Pages.Last();
            var next = new Button
            {
                Text = "▶",
                TextColor = enable_next ? Color.FromHex("#4CAF50") : Color.Default,
                BackgroundColor = Color.Transparent,
            };

            if (enable_next)
            {
                next.Clicked += async (s, e) =>
                {
                    searchResults.CurrentPage++;
                    await Navigation.PushAsync(new SearchResultsPage(searchResults));
                };
            }

            pages_container.Children.Add(next);
        }
    }   
}