using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using Tatoeba.Mobile.Services;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SentenceDetailPage : ContentPage
    {
        SentenceDetailViewModel viewModel;

        public SentenceDetailPage(SentenceDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
            this.viewModel.Error += ViewModel_Error;
        }

        private async void ViewModel_Error(object sender, ErrorEventArgs e)
        {
            if (e.Status == TatoebaStatus.InvalidSession)
            {
                await MainService.ClearCookiers();
                Application.Current.MainPage = new LoginPage();
            }
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Contribution item)
                || item.Id == viewModel.ItemId)
            {
                return;
            }        

            await Navigation.PushAsync(new SentenceDetailPage(new SentenceDetailViewModel(item.Id)));

            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            if(viewModel.Original == null)
            {
                return;
            }

            var target = new NewTranslationPage(viewModel.Original);

            target.ViewModel.Save += ViewModel_Save;  

            await Navigation.PushAsync(target);
        }
    

        private async void ViewModel_Save(object sender, EventArgs e)
        {
            viewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.GroupedCells.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }

    public class SentenceDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SentenceTemplate { get; set; }
        public DataTemplate LogTemplate { get; set; }
        public DataTemplate CommentTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item == null)
                return null;

            switch (item)
            {
                case Contribution _:
                    return SentenceTemplate;
                case Log _:
                    return LogTemplate;
                case Comment _:
                    return CommentTemplate;
                default:
                    throw new Exception("");
            }
        }
    }
}