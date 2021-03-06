﻿using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContributionsPage : TatoebaContentPage<ContributionsViewModel>
    {
        public ContributionsPage()
        {
            InitializeComponent();
            refreshToolbarItem.Text = Resx.AppResources.Refresh;

            ViewModel = new ContributionsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            if (!(args.SelectedItem is Contribution item))
                return;

            await Navigation.PushAsync(new SentenceDetailPage(item.Id));
            ItemsListView.SelectedItem = null;
        }
    }
}