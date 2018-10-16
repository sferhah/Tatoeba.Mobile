using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using System.Linq;
using Tatoeba.Mobile.Services;
using System.Threading.Tasks;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage(SearchResults searchResults)
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel(searchResults);
            BuildPageButtons(searchResults);
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

            bool enable_previous = searchResults.Request.Page.ToString() != searchResults.Pages.First();
            var previous = new Button
            {
                Text = "◀",
                TextColor = enable_previous ? Color.FromHex("#4CAF50") : Color.Gray,
                BackgroundColor = Color.Transparent,
            };

            if (enable_previous)
            {
                previous.Clicked += async (s, e) => await ExecuteSearchCommand(searchResults.Request.Page - 1);                
            }

            pages_container.Children.Add(previous);

            foreach (var page in searchResults.Pages)
            {
                if (page == "...")
                {
                    var label = new Label
                    {
                        Text = page,
                        BackgroundColor = Color.Transparent,
                        TextColor = Color.Default,
                    };
                    pages_container.Children.Add(label);
                }
                else
                {
                    bool enable_button = page != searchResults.Request.Page.ToString();

                    var button = new Button
                    {
                        Text = page,
                        BackgroundColor = enable_button ? Color.FromHex("#EEEEEE") : Color.FromHex("#4CAF50"),
                        TextColor = enable_button ? Color.FromHex("#757575") : Color.White,
                    };
            
                    if (enable_button)
                    {
                        button.Clicked += async (s, e) =>  await ExecuteSearchCommand(int.Parse(page));
                    }

                    pages_container.Children.Add(button);
                }
            }

            bool enable_next = searchResults.Request.Page.ToString() != searchResults.Pages.Last();
            var next = new Button
            {
                Text = "▶",
                TextColor = enable_next ? Color.FromHex("#4CAF50") : Color.Gray,
                BackgroundColor = Color.Transparent,
            };

            if (enable_next)
            {
                next.Clicked += async (s, e) => await ExecuteSearchCommand(searchResults.Request.Page + 1);                
            }

            pages_container.Children.Add(next);
        }
   
        public async Task ExecuteSearchCommand(int page)
        {
            ViewModel.searchResults.Request.Page = page;

            var response = await MainService.SearchAsync(ViewModel.searchResults.Request);

            if (response.Status == TatoebaStatus.Success)
            {
                await Navigation.PushAsync(new SearchResultsPage(response.Content));
            }
        }
    }
}