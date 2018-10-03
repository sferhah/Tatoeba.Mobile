using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using Tatoeba.Mobile.Services;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContributionsPage : ContentPage
    {
        ContributionsViewModel viewModel;

        public ContributionsPage()
        {
            InitializeComponent();
          
            BindingContext = viewModel = new ContributionsViewModel();
            viewModel.Error += ViewModel_Error;
        }

        private async void ViewModel_Error(object sender, ErrorEventArgs e)
        {
            if(e.Status == TatoebaStatus.InvalidSession)
            {
                await MainService.ClearCookiers();
                Application.Current.MainPage = new LoginPage();
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Contribution item))
                return;         

            await Navigation.PushAsync(new SentenceDetailPage(new SentenceDetailViewModel(item.Id)));
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
          //  await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}