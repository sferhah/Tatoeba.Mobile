using System;
using Tatoeba.Mobile.Services;
using Tatoeba.Mobile.ViewModels;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Views
{   
    public class TatoebaContentPage<T> : ContentPage where T : BaseViewModel
    {
        public TatoebaContentPage()
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Log out",
                Command = new Command(() => LogOut()),
            });
        }

        private T _ViewModel;
        public T ViewModel
        {
            get => _ViewModel;
            protected set
            {
                BindingContext = _ViewModel = value;

                if(_ViewModel != null)
                {
                    _ViewModel.Error += ViewModel_Error;
                }
            }
        }

        private void ViewModel_Error(object sender, ErrorEventArgs e)
        {
            if (e.Status == TatoebaStatus.InvalidSession)
            {
                LogOut();
            }
        }    

        protected async void LogOut()
        {
            await MainService.ClearCookiers();
            Application.Current.MainPage = new LoginPage();
        }

    }
}
