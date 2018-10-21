using Android.Content;
using Tatoeba.Mobile.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace Tatoeba.Mobile.Droid.Renderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.SetMinHeight(0);
                Control.SetMinimumHeight(0);
                Control.SetMinWidth(0);
                Control.SetMinimumWidth(0);                
            }
        }
       
    }
}