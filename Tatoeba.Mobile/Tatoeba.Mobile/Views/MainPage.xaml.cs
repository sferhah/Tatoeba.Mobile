using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
            CurrentPageChanged += MainPage_CurrentPageChanged;

            On<Android>().SetToolbarPlacement(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom);
            On<Android>().SetBarSelectedItemColor(Color.FromHex("#4CAF50"));
            //    On<Android>().SetBarItemColor(Color.Red);

           // On<Windows>().SetToolbarPlacement(Xamarin.Forms.PlatformConfiguration.WindowsSpecific.ToolbarPlacement.Bottom);
            On<Windows>().SetHeaderIconsEnabled(true);

            //On<Windows>().SetBarSelectedItemColor(Color.Green);
            //    On<Android>().SetBarItemColor(Color.Red);

            //    BarBackgroundColor = "#2196F3"
            //        android: TabbedPage.BarItemColor = "#66FFFFFF"
            //android: TabbedPage.BarSelectedItemColor = "White"


            ((TatoebaContentPage)(((NavigationPage)CurrentPage).RootPage)).OnShow();

        }


        private async void MainPage_CurrentPageChanged(object sender, System.EventArgs e)
        {
            await CurrentPage.Navigation.PopToRootAsync();
            var page = (TatoebaContentPage)(((NavigationPage)CurrentPage).RootPage);
            page.OnShow();
        }
    }
}