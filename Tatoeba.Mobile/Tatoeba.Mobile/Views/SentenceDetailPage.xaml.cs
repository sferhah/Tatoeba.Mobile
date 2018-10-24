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
        public SentenceDetailPage() : this(null) { }

        public SentenceDetailPage(string itemId)
        {
            InitializeComponent();
            ViewModel = new SentenceDetailViewModel(itemId);
            ViewModel.Loaded += ViewModel_Loaded;
        }

        private void ViewModel_Loaded(object sender, SentenceLoadedEventArgs e)
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Add translation",
                Icon = "add.png",
                Command = new Command(() => AddItem_Clicked()),
            });

            if (e.IsEditable)
            {
                ToolbarItems.Add(new ToolbarItem
                {
                    Text = "Edit",
                    Icon = "edit.png",
                    Command = new Command(() => GoToEditPage()),
                });
            }
        }

        protected async void GoToEditPage()
        {        
            var target = new EditSentencePage(ViewModel.Original);

            target.ViewModel.Save += ViewModel_Save;

            await Navigation.PushModalAsync(new NavigationPage(target));
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if ((args.SelectedItem is ExpandToggler))
            {
                ItemsListView.SelectedItem = null;
                ViewModel.ToggleCommand.Execute(null);
                return;
            }

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

        async void AddItem_Clicked()
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
            ViewModel.RefreshOriginal();
        }       
    }

    public class SentenceDetailTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SentenceTemplate { get; set; }
        public DataTemplate ExpandTogglerTemplate { get; set; }
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
                case ExpandToggler _:
                    return ExpandTogglerTemplate;
                default:
                    throw new Exception("");
            }
        }
    }
}