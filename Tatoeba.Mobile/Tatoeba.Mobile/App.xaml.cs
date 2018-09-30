using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Views;
using Tatoeba.Mobile.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Tatoeba.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();


            //MainPage = new MainPage();
            MainPage = new LoadingPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
