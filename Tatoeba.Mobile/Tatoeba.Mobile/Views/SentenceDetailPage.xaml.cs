using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SentenceDetailPage : TatoebaContentPage<SentenceDetailViewModel>
    {

        public SentenceDetailPage(string itemId)
        {
            InitializeComponent();
            ViewModel = new SentenceDetailViewModel(itemId);
            ViewModel.ShowEditAction += ViewModel_ShowEditAction;
        }

        private void ViewModel_ShowEditAction(object sender, EventArgs e)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Edit",
                Icon = "edit.png",
                Command = new Command(() => GoToEditPage()),
            });
        }

        protected async void GoToEditPage()
        {        
            var target = new EditSentencePage(ViewModel.Original);

            target.ViewModel.Save += ViewModel_Save;

            await Navigation.PushModalAsync(new NavigationPage(target));
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Contribution item))
            {
                return;
            }

            if (item.Id == ViewModel.ItemId)
            {
               // GoToEditPage();
              //  ItemsListView.SelectedItem = null;
                return;
            }

            await Navigation.PushAsync(new SentenceDetailPage(item.Id));

            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            if (ViewModel.Original == null)
            {
                return;
            }

            var target = new NewTranslationPage(ViewModel.Original);

            target.ViewModel.Save += ViewModel_Save;

            await Navigation.PushModalAsync(new NavigationPage(target));
        }
    

        private async void ViewModel_Save(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel.GroupedCells.Count == 0)
                ViewModel.LoadItemsCommand.Execute(null);
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