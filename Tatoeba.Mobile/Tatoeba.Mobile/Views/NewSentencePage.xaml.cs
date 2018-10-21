using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewSentencePage : TatoebaContentPage<NewSentenceViewModel>
    {        
        public NewSentencePage(string iso)
        {
            InitializeComponent();
            ViewModel = new NewSentenceViewModel(iso);

            ViewModel.Save += async (sender, e) => await Navigation.PopModalAsync();
            ViewModel.Cancel += async (sender, e) => await Navigation.PopModalAsync();
        }
    }
}