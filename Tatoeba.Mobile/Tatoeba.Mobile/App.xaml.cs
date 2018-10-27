using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tatoeba.Mobile.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Tatoeba.Mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            Resx.AppResources.SetIso(DependencyService.Get<Resx.ILocalize>().ThreeLetterISOLanguageName);
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
