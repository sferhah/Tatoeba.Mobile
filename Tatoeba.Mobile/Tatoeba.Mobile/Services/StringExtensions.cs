using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tatoeba.Mobile.Models;

namespace Tatoeba.Mobile.Services
{ 

    public static class StringExtensions
    {
        public static Language ToLanguage(this string @this)
        {   
            var matches = new Regex("'.*?'").Matches(@this).Cast<Match>().Select(x => x.ToString()).Select(mystr => mystr.Substring(1, mystr.Length - 2)).ToList();

            return matches.Count != 3 ? null : new Language
            {
                Iso = matches[0],
                Label = matches[2],
            };
        }

        public static string Substring(this string @this, string from = null, string until = null, StringComparison comparison = StringComparison.InvariantCulture)
        {
            var fromLength = (from ?? string.Empty).Length;
            var startIndex = !string.IsNullOrEmpty(from)
                ? @this.IndexOf(from, comparison) + fromLength
                : 0;

            if (startIndex < fromLength)
            {
                throw new ArgumentException("from: Failed to find an instance of the first anchor");
            }

            var endIndex = !string.IsNullOrEmpty(until)
            ? @this.IndexOf(until, startIndex, comparison)
            : @this.Length;

            if (endIndex < 0)
            {
                throw new ArgumentException("until: Failed to find an instance of the last anchor");
            }

            return @this.Substring(startIndex, endIndex - startIndex);
        }

        public static string UrlEncode(this string str)
        {
            if(str == null)
            {
                return string.Empty;
            }

            int limit = 32766;    // 32766 is the longest string allowed in Uri.EscapeDataString()

            if (str.Length <= limit)
            {
                return Uri.EscapeDataString(str);
            }

            StringBuilder sb = new StringBuilder(str.Length);
            int portions = str.Length / limit;

            for (int i = 0; i <= portions; i++)
            {
                if (i < portions)
                    sb.Append(Uri.EscapeDataString(str.Substring(limit * i, limit)));
                else
                    sb.Append(Uri.EscapeDataString(str.Substring(limit * i)));
            }

            return sb.ToString();
        }
    }
}
