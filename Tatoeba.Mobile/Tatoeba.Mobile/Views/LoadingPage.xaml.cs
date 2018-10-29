using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
            reload_button.Text = " " + Resx.AppResources.Reload + " ";
            label.Text = Resx.AppResources.LoadingResources;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();          

            Reload_Clicked(null, null);
        }

        private async void Reload_Clicked(object sender, EventArgs e)
        {
            loadingControl.IsVisible = true;
            reload_button.IsVisible = false;
            label.IsVisible = true;

            bool hasSession = false; ;

            try
            {
                hasSession = await Services.MainService.InitAsync();                
            }
            catch(Exception ex)
            {
                await DisplayAlert(Resx.AppResources.Error, Resx.AppResources.ErrorLoadingResources + " " + ex.Message, Resx.AppResources.Ok);
                loadingControl.IsVisible = false;
                reload_button.IsVisible = true;
                label.IsVisible = false;
                return; 
            }

            loadingControl.IsVisible = false;

            Application.Current.MainPage = hasSession ? new MainPage() : (Page)new LoginPage();
        }
    }

}