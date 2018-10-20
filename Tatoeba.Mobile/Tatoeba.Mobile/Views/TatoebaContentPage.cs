using System.Threading.Tasks;
using Tatoeba.Mobile.Services;
using Tatoeba.Mobile.ViewModels;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Views
{
    public abstract class TatoebaContentPage : ContentPage 
    {
        public TatoebaContentPage()
        {
            ToolbarItems.Add(new ToolbarItem
            {
                Text = "Log out",
                Priority = 1,
                Order = ToolbarItemOrder.Secondary,
                Command = new Command(() => LogOut()),
            });
        }

        protected async void LogOut()
        {
            await MainService.ClearCookiers();
            Application.Current.MainPage = new LoginPage();
        }

        public abstract void OnShow();
        
    }

    public class TatoebaContentPage<T> : TatoebaContentPage where T : BaseViewModel
    {
        public TatoebaContentPage() : base() { }

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Navigation.NavigationStack.Count == 1) //Disables OnShow for Root Pages
            {
                return;
            }

            OnShow();
        }

        public override void OnShow()
        {
            ViewModel.OnShow();
        }
    }
}
