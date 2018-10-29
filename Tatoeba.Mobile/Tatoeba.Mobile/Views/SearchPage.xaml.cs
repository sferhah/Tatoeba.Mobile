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

            fromLabel.Text = Resx.AppResources.From;
            toLabel.Text = Resx.AppResources.To;
            isOrphanOriginLabel.Text = Resx.AppResources.IsOrphan;
            isOrphanTransLabel.Text = Resx.AppResources.IsOrphan;
            isUnapprovedOriginLabel.Text = Resx.AppResources.IsUnapproved;
            isUnapprovedTransLabel.Text = Resx.AppResources.IsUnapproved;
            hasAudioOriginLabel.Text = Resx.AppResources.HasAudio;
            hasAudioTransLabel.Text = Resx.AppResources.HasAudio;

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