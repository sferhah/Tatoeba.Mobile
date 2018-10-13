using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.XPath;
using Tatoeba.Mobile.Models;

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

            if(typeof(T) != typeof(string)
                || result == null
                || xpath.Action == XpathActionConfig.None)
            {
                return (T)result;
            }

            var stringResult = (string)result;


            switch(xpath.Action)
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


    public enum TatoebaStatus
    {
        Success,
        InvalidSession,
        Error,
    }

    public class TatoebaResponse<T>
    {
        public T Content;
        public TatoebaStatus Status;
    }

    public class TatoebaScraper
    {   
        readonly static List<string> directions = new List<string>
        {
            "ltr",
            "rtl"
        };
        readonly static List<string> translationTypes = new List<string>
        {
            "navigationIcon mainSentence",
            "navigationIcon directTranslation",
            "navigationIcon indirectTranslation"
        };
        readonly static List<string> contribTypes = new List<string>
        {
            "md-2-line sentence-insert",
            "md-2-line sentence-update",
            "md-2-line sentence-obsolete",
            "md-2-line sentence-delete",
            "md-2-line link-insert",
            "md-2-line link-delete",
        };


        public static bool IsSessionValid(string respStr) => respStr.Contains("li id=\"profile\"");

        public static TatoebaResponse<Contribution[]> ParseContribs(string result)
        {
            if (!IsSessionValid(result))
            {
                return new TatoebaResponse<Contribution[]>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }

            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            List<Contribution> sentences = new List<Contribution>();


            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathContribConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                sentences.Add(new Contribution
                {
                    Text = nav.Evaluate<string>(XpathContribConfig.TextPath),
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathContribConfig.DirectionPath)),
                    DateText = nav.Evaluate<string>(XpathContribConfig.DateTextPath),
                    Id = nav.Evaluate<string>(XpathContribConfig.IdPath)?.Split('/').Last(),
                    Language = new Language { Iso = nav.Evaluate<string>(XpathContribConfig.LanguagePath) },
                    ContribType = (ContribType)contribTypes.IndexOf(nav.Evaluate<string>(XpathContribConfig.ContribTypePath)),
                });
            }

            foreach (var item in sentences)
            {
                item.Language = MainService.Languages.Where(x => x.Iso == item.Language.Iso).SingleOrDefault();
            }

            return new TatoebaResponse<Contribution[]>
            {
                Content = sentences.ToArray(),
            };
        }

        public static TatoebaResponse<SentenceDetail> ParseSetenceDetail(string result)
        {
            if (!IsSessionValid(result))
            {
                return new TatoebaResponse<SentenceDetail>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }

            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            SentenceDetail setenceDetails = new SentenceDetail
            {
                IsEditable = doc.CreateNavigator().Evaluate<bool>(XpathTranslationConfig.IsEditablePath)
            };

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathTranslationConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();                

                setenceDetails.Sentences.Add(new Contribution
                {
                    Text = nav.Evaluate<string>(XpathTranslationConfig.TextPath),
                    Id = nav.Evaluate<string>(XpathTranslationConfig.IdPath),
                    Language = new Language { Iso = nav.Evaluate<string>(XpathTranslationConfig.LanguagePath) },
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathTranslationConfig.DirectionPath)),
                    TranslationType = (TranslationType)translationTypes.IndexOf(nav.Evaluate<string>(XpathTranslationConfig.TranslationTypePath))
                });
            }           

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathLogConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Logs.Add(new Log
                {
                    Text = nav.Evaluate<string>(XpathLogConfig.TextPath),
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathLogConfig.DateTextPath)),
                    DateText = nav.Evaluate<string>(XpathLogConfig.DateTextPath),
                    ContribType = (ContribType)contribTypes.IndexOf(nav.Evaluate<string>(XpathLogConfig.ContribTypePath)),
                });
            }

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathCommentConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Comments.Add(new Comment
                {
                    Username = nav.Evaluate<string>(XpathCommentConfig.UsernamePath),
                    Content = nav.Evaluate<string>(XpathCommentConfig.ContentPath),
                    DateText = nav.Evaluate<string>(XpathCommentConfig.DateTextPath),
                });
            }

            foreach (var item in setenceDetails.Sentences)
            {
                item.Language = MainService.Languages.Where(x => x.Iso == item.Language.Iso).SingleOrDefault();
            }

            return new TatoebaResponse<SentenceDetail>
            {
                Content = setenceDetails,
            };            
        }
    }
}
