using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTranslationPage : ContentPage
    {
        public NewTranslationViewModel ViewModel { get; private set; }
        
        public NewTranslationPage(Contribution original)
        {
            InitializeComponent();
            BindingContext = ViewModel = new NewTranslationViewModel(original);

            ViewModel.Save += ViewModel_Save;
            ViewModel.Cancel += ViewModel_Cancel;
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