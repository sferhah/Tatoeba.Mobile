using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Tatoeba.Mobile.Views.MainPage), typeof(Tatoeba.Mobile.UWP.Renderers.MainTabPageRenderer))]
namespace Tatoeba.Mobile.UWP.Renderers
{
    public class MainTabPageRenderer : TabbedPageRenderer
    {
        private Xamarin.Forms.Page _prevPage;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            Control.Tapped += Control_Tapped;
            _prevPage = Control.SelectedItem as Xamarin.Forms.Page;
        }

        private void Control_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (e.OriginalSource is Windows.UI.Xaml.FrameworkElement src
                 && (src.Name == "TabbedPageHeaderTextBlock" || src.Name == "TabbedPageHeaderImage")
                 && Element is Tatoeba.Mobile.Views.MainPage)
            {
                var newPage = src.DataContext as Xamarin.Forms.Page;
                if (newPage == _prevPage)
                {
                    var page = Element as Tatoeba.Mobile.Views.MainPage;
                    page.CurrentPage.Navigation.PopToRootAsync();
                }
                _prevPage = newPage;
            }
        }       
    }
}
