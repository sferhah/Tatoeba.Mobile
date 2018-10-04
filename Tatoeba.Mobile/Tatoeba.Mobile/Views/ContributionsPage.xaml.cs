using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContributionsPage : TatoebaContentPage
    {

        public ContributionsPage()
        {
            InitializeComponent();          
            ViewModel = new ContributionsViewModel();
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

            if ((ViewModel as ContributionsViewModel).Items.Count == 0)
                (ViewModel as ContributionsViewModel).LoadItemsCommand.Execute(null);
        }
    }
}