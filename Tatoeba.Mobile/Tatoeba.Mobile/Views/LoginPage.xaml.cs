using System;
using Tatoeba.Mobile.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            BindingContext = this;
            InitializeComponent();

            usernameEntry.Text = Resx.AppResources.Username;
            passwordEntry.Text = Resx.AppResources.Password;

            usernameEntry.Completed += (s, e) => passwordEntry.Focus();
            passwordEntry.Completed += (s, e) => Login(null, null);
        }       

        private async void Login(object sender, EventArgs e)
        {
            loginButton.IsVisible = false;
            loader.IsVisible = true;

            var resp = await MainService.LogInAsync(usernameEntry.Text, passwordEntry.Text);

            if(resp.Status != TatoebaStatus.Success)
            {
                await DisplayAlert(Resx.AppResources.Error, Resx.AppResources.ErrorOccured + " " + resp.Error, Resx.AppResources.Ok);

                loginButton.IsVisible = true;
                loader.IsVisible = false;

                return;
            }

            if (!resp.Content)
            {
                await DisplayAlert(Resx.AppResources.Error, Resx.AppResources.InvalidCredentials, Resx.AppResources.Ok);

                loginButton.IsVisible = true;
                loader.IsVisible = false;

                return;
            }

            Application.Current.MainPage = new MainPage();
        }
    }
}