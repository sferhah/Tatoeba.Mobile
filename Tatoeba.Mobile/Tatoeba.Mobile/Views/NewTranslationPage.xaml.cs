using System;
using Xamarin.Forms;
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

            (ViewModel as NewTranslationViewModel).Save += ViewModel_Save;
            (ViewModel as NewTranslationViewModel).Cancel += ViewModel_Cancel;
        }

        private async void ViewModel_Save(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void ViewModel_Cancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}