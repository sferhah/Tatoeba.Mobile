using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            usernameEntry.Completed += (s, e) => passwordEntry.Focus();
            passwordEntry.Completed += (s, e) => Login(null, null);
        }       

        private async void Login(object sender, EventArgs e)
        {
            loginButton.IsVisible = false;
            loader.IsVisible = true;

            var success = await MainService.LogInAsync(usernameEntry.Text, passwordEntry.Text);

            if(success)
            {
                Application.Current.MainPage = new MainPage();
            }
            else
            {
                await DisplayAlert("Error", "Invalid credatials", "Ok"); 
            }

            loginButton.IsVisible = true;
            loader.IsVisible = false;
        }
    }
}