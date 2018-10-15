using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;
using System.Collections.Generic;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : TatoebaContentPage<SearchResultsViewModel>
    {
        public SearchResultsPage(List<SentenceSet> searchResults)
        {
            InitializeComponent();
            ViewModel = new SearchResultsViewModel(searchResults);         
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {

        }

        async void AddItem_Clicked()
        {

        }
    }   
}