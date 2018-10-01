using System;
using System.Globalization;
using Tatoeba.Mobile.Models;
using Xamarin.Forms;

namespace Tatoeba.Mobile.Converters
{

    public class TranslationTypeToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TranslationType translationType))
            {
                return null;
            }

            switch (translationType)
            {
                case TranslationType.Original:
                    return "original_sentence.png";
                case TranslationType.Direct:
                    return "direct_translation.png";
                case TranslationType.Indirect:
                    return "indirect_translation.png";
                default:
                    return "indirect_translation.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
