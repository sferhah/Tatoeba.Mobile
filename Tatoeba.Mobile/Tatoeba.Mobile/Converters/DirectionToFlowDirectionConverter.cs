using System;
using System.Globalization;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{
    public class DirectionToFlowDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Direction direction))
            {
                return FlowDirection.MatchParent;
            }

            switch (direction)
            {
                case Direction.LeftToRight:
                    return FlowDirection.LeftToRight;
                case Direction.RightToLeft:
                    return FlowDirection.RightToLeft;                
                default:
                    return FlowDirection.MatchParent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
