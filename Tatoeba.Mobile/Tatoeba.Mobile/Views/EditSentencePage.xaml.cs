﻿using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.ViewModels;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditSentencePage : TatoebaContentPage<EditSentenceViewModel>
    {        
        public EditSentencePage(Contribution original)
        {
            InitializeComponent();

            cancelToolbarItem.Text = Resx.AppResources.Cancel;
            validateToolbarItem.Text = Resx.AppResources.Validate;

            ViewModel = new EditSentenceViewModel(original);

            ViewModel.Save += async (sender, e) => await Navigation.PopModalAsync();
            ViewModel.Cancel += async (sender, e) => await Navigation.PopModalAsync();
        }
    }
}