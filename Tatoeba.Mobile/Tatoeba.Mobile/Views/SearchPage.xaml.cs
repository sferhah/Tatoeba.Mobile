using System;
using Tatoeba.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : TatoebaContentPage<SearchViewModel>
    {
        public SearchPage()
        {
            InitializeComponent();
        }
    }
}