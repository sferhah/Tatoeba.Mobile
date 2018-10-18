using HtmlAgilityPack;
using System.Web;
using System.Xml.XPath;

namespace Tatoeba.Mobile.Services
{
    public static class HtmlAgilityPackExtensions
    {
        public static HtmlNodeCollection SelectNodesOrEmpty(this HtmlNode @this, string xpath)
            => @this.SelectNodes(xpath) ?? new HtmlNodeCollection(null);

        public static T Evaluate<T>(this XPathNavigator @this, string xpath)
        {
            var result = @this.Evaluate(xpath);
            if (result == null)
            {
                return default(T);
            }
            return (T)result;
        }

        public static T Evaluate<T>(this XPathNavigator @this, XpathPathConfig xpath)
        {
            var result = @this.Evaluate(xpath.Path);
            if (result == null)
            {
                return default(T);
            }

            if (typeof(T) != typeof(string)
                || result == null
                || xpath.Action == XpathActionConfig.None)
            {
                return (T)result;
            }

            var stringResult = (string)result;


            switch (xpath.Action)
            {
                case XpathActionConfig.Trim:
                    stringResult = stringResult.Trim();
                    break;
                case XpathActionConfig.HtmlDecode:
                    stringResult = HttpUtility.HtmlDecode(stringResult);
                    break;
                case XpathActionConfig.HtmlDecodeAndTrim:
                    stringResult = HttpUtility.HtmlDecode(stringResult).Trim();
                    break;
            }

            return (T)(object)stringResult;
        }
    }
}
