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

        public static T EvaluateAs<T>(this XPathNavigator @this, string xpath)
        {
            var result = @this.Evaluate(xpath);
            if (result == null)
            {
                return default(T);
            }
            return (T)result;
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
                    Text = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathContribConfig.TextPath)).Trim(),
                    Direction = (Direction)directions.IndexOf(nav.EvaluateAs<string>(XpathContribConfig.DirectionPath)),
                    DateText = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathContribConfig.DateTextPath)).Trim(),
                    Id = nav.EvaluateAs<string>(XpathContribConfig.IdPath)?.Split('/').Last(),
                    Language = new Language { Iso = nav.EvaluateAs<string>(XpathContribConfig.LanguagePath) },
                    ContribType = (ContribType)contribTypes.IndexOf(nav.EvaluateAs<string>(XpathContribConfig.ContribTypePath)),
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
                IsEditable = doc.CreateNavigator().EvaluateAs<bool>(XpathTranslationConfig.IsEditablePath)
            };

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathTranslationConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();                

                setenceDetails.Sentences.Add(new Contribution
                {
                    Text = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathTranslationConfig.TextPath)).Trim(),
                    Id = nav.EvaluateAs<string>(XpathTranslationConfig.IdPath),
                    Language = new Language { Iso = nav.EvaluateAs<string>(XpathTranslationConfig.LanguagePath) },
                    Direction = (Direction)directions.IndexOf(nav.EvaluateAs<string>(XpathTranslationConfig.DirectionPath)),
                    TranslationType = (TranslationType)translationTypes.IndexOf(nav.EvaluateAs<string>(XpathTranslationConfig.TranslationTypePath))
                });
            }           

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathLogConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Logs.Add(new Log
                {
                    Text = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathLogConfig.TextPath)).Trim(),
                    Direction = (Direction)directions.IndexOf(nav.EvaluateAs<string>(XpathLogConfig.DateTextPath)),
                    DateText = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathLogConfig.DateTextPath)).Trim(),
                    ContribType = (ContribType)contribTypes.IndexOf(nav.EvaluateAs<string>(XpathLogConfig.ContribTypePath)),
                });
            }

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathCommentConfig.ItemsPath))
            {
                var nav = node.CreateNavigator();

                setenceDetails.Comments.Add(new Comment
                {
                    Username = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathCommentConfig.UsernamePath)).Trim(),
                    Content = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathCommentConfig.ContentPath)).Trim(),
                    DateText = HttpUtility.HtmlDecode(nav.EvaluateAs<string>(XpathCommentConfig.DateTextPath)).Trim(),
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
