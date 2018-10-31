using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tatoeba.Mobile.PlatformSpecific;
using Tatoeba.Mobile.Storage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UiLanguagesPage : ContentPage
    {
        List<(string Iso, string Label)> UiLanguages = new List<(string, string)>
        {
            (null, "Default"),
            ("arb", "عربية فصحى حديثة"),
            ("arq", "عربية دزيرية"),            
            ("deu", "Deutsch"),
            ("eng", "English"),
            ("heb", "עברית חדשה"),
            ("fra", "Français"),
            ("kab", "Taqbaylit"),
            ("mlt", "Malti"),
            ("rus", "Русский"),
        };

        public UiLanguagesPage()
        {
            InitializeComponent();
            cancelToolbarItem.Text = Resx.AppResources.Cancel;


            ItemsListView.ItemsSource = UiLanguages.Select(x=> x.Label).ToList();
        }

        private async void cancelToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }  

        private void search_entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox?.Text))
            {
                ItemsListView.ItemsSource = UiLanguages.Select(x => x.Label).ToList();
            }
            else
            {
                ItemsListView.ItemsSource = UiLanguages.Select(x => x.Label)
                .Where(x => CultureInfo.InvariantCulture.CompareInfo.IndexOf(Convert.ToString(x), searchBox.Text, CompareOptions.IgnoreCase) >= 0)
                .ToList();
            }
        }

        private async void ItemsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            LocalSettings.LastUiIso = UiLanguages.Single(x => x.Label == (string)e.SelectedItem).Iso ?? Localize.ThreeLetterISOLanguageName;

            Resx.AppResources.SetIso(LocalSettings.LastUiIso);

            Application.Current.MainPage = new MainPage();
        }
    }
}