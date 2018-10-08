using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tatoeba.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingControl : ContentView
    {
        public LoadingControl()
        {
            InitializeComponent();
        }
    }
}