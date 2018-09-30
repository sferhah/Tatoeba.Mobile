using System;
using System.Globalization;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{

    public class TranslationTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TranslationType translationType))
            {
                return string.Empty;
            }

            switch (translationType)
            {
                case TranslationType.Original:
                    return "#";
                default:
                    return "⯈";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TranslationTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TranslationType translationType))
            {
                return "Transparent";
            }

            switch (translationType)
            {
                case TranslationType.Original:
                    return "Green";
                case TranslationType.Direct:
                    return "Cyan";
                case TranslationType.Indirect:
                    return "LightGray";
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
