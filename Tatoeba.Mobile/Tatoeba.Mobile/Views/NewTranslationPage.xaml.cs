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
        NewTranslationViewModel viewModel;
        
        public NewTranslationPage(Contribution original)
        {
            InitializeComponent();
            BindingContext = viewModel = new NewTranslationViewModel(original);

            viewModel.Save += ViewModel_Save;
            viewModel.Cancel += ViewModel_Cancel;
        }

        private async void ViewModel_Save(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void ViewModel_Cancel(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}