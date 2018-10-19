namespace Tatoeba.Mobile.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            Rg.Plugins.Popup.Popup.Init();
            LoadApplication(new Tatoeba.Mobile.App());
        }
    }
}
