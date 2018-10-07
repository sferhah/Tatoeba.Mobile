using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTranslationPage : TatoebaContentPage<NewTranslationViewModel>
    {        
        public NewTranslationPage(Contribution original)
        {
            InitializeComponent();
            ViewModel = new NewTranslationViewModel(original);

            ViewModel.Save += async (sender, e) => await Navigation.PopAsync();
            ViewModel.Cancel += async (sender, e) => await Navigation.PopAsync();
        } 
    }
}