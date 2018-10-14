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

        private static XpathConfig _XpathConfig;
        public static XpathConfig XpathConfig
        {
            get => _XpathConfig ?? new XpathConfig();
            set => _XpathConfig = value;
        }

        public static bool IsSessionValid(HtmlDocument doc) => doc.CreateNavigator().Evaluate<bool>(XpathConfig.LoginConfig.SuccessPath);

        public static TatoebaResponse<Contribution[]> ParseContribs(string result)
        {
            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            if (!IsSessionValid(doc))
            {
                return new TatoebaResponse<Contribution[]>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }

            List<Contribution> sentences = new List<Contribution>();


            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.ContribConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                sentences.Add(new Contribution
                {
                    Text = nav.Evaluate<string>(XpathConfig.ContribConfig.TextPath),
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathConfig.ContribConfig.DirectionPath)),
                    DateText = nav.Evaluate<string>(XpathConfig.ContribConfig.DateTextPath),
                    Id = nav.Evaluate<string>(XpathConfig.ContribConfig.IdPath)?.Split('/').Last(),
                    Language = new Language { Iso = nav.Evaluate<string>(XpathConfig.ContribConfig.LanguagePath) },
                    ContribType = (ContribType)contribTypes.IndexOf(nav.Evaluate<string>(XpathConfig.ContribConfig.ContribTypePath)),
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
            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            if (!IsSessionValid(doc))
            {
                return new TatoebaResponse<SentenceDetail>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }

            SentenceDetail setenceDetails = new SentenceDetail
            {
                IsEditable = doc.CreateNavigator().Evaluate<bool>(XpathConfig.SentenceDetailConfig.TranslationConfig.IsEditablePath)
            };

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.SentenceDetailConfig.TranslationConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();                

                setenceDetails.Sentences.Add(new Contribution
                {
                    Text = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.TextPath),
                    Id = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.IdPath),
                    Language = new Language { Iso = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.LanguagePath) },
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.DirectionPath)),
                    TranslationType = (TranslationType)translationTypes.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.TranslationTypePath))
                });
            }           

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.SentenceDetailConfig.LogConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Logs.Add(new Log
                {
                    Text = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.LogConfig.TextPath),
                    Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.LogConfig.DateTextPath)),
                    DateText = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.LogConfig.DateTextPath),
                    ContribType = (ContribType)contribTypes.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.LogConfig.ContribTypePath)),
                });
            }

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.SentenceDetailConfig.CommentConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Comments.Add(new Comment
                {
                    Username = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.CommentConfig.UsernamePath),
                    Content = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.CommentConfig.ContentPath),
                    DateText = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.CommentConfig.DateTextPath),
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
