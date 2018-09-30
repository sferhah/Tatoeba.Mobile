using System.ComponentModel;
using Tatoeba.Mobile.NativeViews;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

using WinColor = Windows.UI.Color;
using WinThickness = Windows.UI.Xaml.Thickness;
using XFColor = Xamarin.Forms.Color;
using XFThickness = Xamarin.Forms.Thickness;

[assembly: ExportRenderer(typeof(UwpProgressRing), typeof(Tatoeba.Mobile.UWP.Renderers.UwpProgressRingRenderer))]
namespace Tatoeba.Mobile.UWP.Renderers
{   
    public class UwpProgressRingRenderer : ViewRenderer<UwpProgressRing, ProgressRing>
    {
        ProgressRing ring;
        protected override void OnElementChanged(ElementChangedEventArgs<UwpProgressRing> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
                return;

            SetNativeControl(ring = new ProgressRing()
            {
                IsActive = true,
                Visibility = Windows.UI.Xaml.Visibility.Visible,
                IsEnabled = true,
                Foreground = Element.ForegroundColor.ToBrush(),
            });
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Width")
            {
                ring.Width = Element.Width;
                return;
            }

            if (e.PropertyName == "Height")
            {
                ring.Width = Element.Height;
                return;
            }

            if (e.PropertyName == "ForegroundColor")
            {
                ring.Foreground = Element.ForegroundColor.ToBrush();
                return;
            }
        }
    }

    internal static class ColorConverter
    {
        public static Brush ToBrush(this Color color)
        {
            return new SolidColorBrush(color.ToMediaColor());
        }
        public static WinColor ToMediaColor(this XFColor color)
        {
            return WinColor.FromArgb((byte)(color.A * 255), (byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255));
        }
    }
}
