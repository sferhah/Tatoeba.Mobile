using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSentencePage : TatoebaContentPage
    {        
        public EditSentencePage(Contribution original)
        {
            InitializeComponent();
            ViewModel = new EditSentenceViewModel(original);

            (ViewModel as EditSentenceViewModel).Save += async (sender, e) => await Navigation.PopAsync();
            (ViewModel as EditSentenceViewModel).Cancel += async (sender, e) => await Navigation.PopAsync();
        }
    }
}