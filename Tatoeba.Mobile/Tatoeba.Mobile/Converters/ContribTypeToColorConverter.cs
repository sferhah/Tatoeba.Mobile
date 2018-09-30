using System;
using System.Globalization;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{
    public class ContribTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ContribType contribType))
            {
                return "Transparent";
            }

            switch (contribType)
            {
                case ContribType.Insert:
                    return "Green";
                case ContribType.Update:
                    return "Yellow";
                case ContribType.Obsolete:
                    return "Gray";
                case ContribType.Delete:
                    return "Red";
                case ContribType.LinkInsert:
                    return "Blue";
                case ContribType.LinkDelete:
                    return "Orange";
                default:
                    return "Transparent";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
