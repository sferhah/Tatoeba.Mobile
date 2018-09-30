using System;
using System.Globalization;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{
    public class DirectionToLayoutOptionsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Direction direction))
            {
                return LayoutOptions.Start;
            }

            switch (direction)
            {
                case Direction.RightToLeft:
                    return LayoutOptions.End;
                default:
                    return LayoutOptions.Start;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
