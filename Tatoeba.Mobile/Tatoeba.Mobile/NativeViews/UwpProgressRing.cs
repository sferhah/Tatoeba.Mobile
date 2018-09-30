using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Tatoeba.Mobile.NativeViews
{
    public class UwpProgressRing : View
    {
        public static readonly BindableProperty ForegroundColorProperty =
                BindableProperty.Create(nameof(ForegroundColor), typeof(Color), typeof(UwpProgressRing), default(Color));

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }
    }
}
