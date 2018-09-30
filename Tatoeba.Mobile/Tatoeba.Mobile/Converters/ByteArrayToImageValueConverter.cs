using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{
    public class ByteArrayToImageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return ImageSource.FromFile("default_miniature.png");
            }

            return ImageSource.FromStream(() => new MemoryStream((byte[])value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
