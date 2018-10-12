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
            CurrentPageChanged += async (s, e) => await CurrentPage.Navigation.PopToRootAsync();

            //   this.BarBackgroundColor = Color.Black;

            On<Android>().SetToolbarPlacement(Xamarin.Forms.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom);
            On<Android>().SetBarSelectedItemColor(Color.Green);
            //    On<Android>().SetBarItemColor(Color.Red);

            //   On<Windows>().SetToolbarPlacement(Xamarin.Forms.PlatformConfiguration.WindowsSpecific.ToolbarPlacement.Bottom);
            //On<Windows>().SetBarSelectedItemColor(Color.Green);
            //    On<Android>().SetBarItemColor(Color.Red);

            //    BarBackgroundColor = "#2196F3"
            //        android: TabbedPage.BarItemColor = "#66FFFFFF"
            //android: TabbedPage.BarSelectedItemColor = "White"
        }  
    }
}