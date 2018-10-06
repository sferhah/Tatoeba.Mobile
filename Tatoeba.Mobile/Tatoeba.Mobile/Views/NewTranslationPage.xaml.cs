using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTranslationPage : TatoebaContentPage
    {        
        public NewTranslationPage(Contribution original)
        {
            InitializeComponent();
            ViewModel = new NewTranslationViewModel(original);

            (ViewModel as NewTranslationViewModel).Save += async (sender, e) => await Navigation.PopAsync();
            (ViewModel as NewTranslationViewModel).Cancel += async (sender, e) => await Navigation.PopAsync();
        } 
    }
}