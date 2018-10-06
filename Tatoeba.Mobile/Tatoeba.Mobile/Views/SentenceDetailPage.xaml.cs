using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using Tatoeba.Mobile.Services;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SentenceDetailPage : TatoebaContentPage
    {

        public SentenceDetailPage(SentenceDetailViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            viewModel.ShowEditAction += ViewModel_ShowEditAction;
        }

        private void ViewModel_ShowEditAction(object sender, EventArgs e)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Edit",
                Command = new Command(() => GoToEditPage()),
            });
        }

        protected async void GoToEditPage()
        {        
            var target = new EditSentencePage((ViewModel as SentenceDetailViewModel).Original);

            (target.ViewModel as EditSentenceViewModel).Save += ViewModel_Save;

            await Navigation.PushAsync(target);
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Contribution item))
            {
                return;
            }

            if (item.Id == (ViewModel as SentenceDetailViewModel).ItemId)
            {
               // GoToEditPage();
              //  ItemsListView.SelectedItem = null;
                return;
            }

            await Navigation.PushAsync(new SentenceDetailPage(new SentenceDetailViewModel(item.Id)));

            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            if ((ViewModel as SentenceDetailViewModel).Original == null)
            {
                return;
            }

            var target = new NewTranslationPage((ViewModel as SentenceDetailViewModel).Original);

            (target.ViewModel as NewTranslationViewModel).Save += ViewModel_Save;  

            await Navigation.PushAsync(target);
        }
    

        private async void ViewModel_Save(object sender, EventArgs e)
        {
            (ViewModel as SentenceDetailViewModel).LoadItemsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if ((ViewModel as SentenceDetailViewModel).GroupedCells.Count == 0)
                (ViewModel as SentenceDetailViewModel).LoadItemsCommand.Execute(null);
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