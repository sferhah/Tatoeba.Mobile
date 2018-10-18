using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
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

            var docNav = doc.CreateNavigator();

            SentenceDetail setenceDetails = new SentenceDetail
            {
                IsEditable = docNav.Evaluate<bool>(XpathConfig.SentenceDetailConfig.TranslationConfig.IsEditablePath),
                PreviousId = docNav.Evaluate<string>("string(//ul//li[@id='prevSentence']/a/@href)")?.Split('/').Last(),
                NextId = docNav.Evaluate<string>("string(//ul//li[@id='nextSentence']/a/@href)")?.Split('/').Last(),
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

        public static int GetPageCount(List<string> input)
        {
            input = input.Distinct().ToList();

            List<int> input_int = new List<int>();

            foreach (var page in input)
            {
                if (int.TryParse(page, out int pageInt))
                {
                    input_int.Add(pageInt);
                }
            }

            return input_int.Any() ? input_int.Max() : 0;
        }

        public static TatoebaResponse<SearchResults> ParseSearchResults(string result)
        {
            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            if (!IsSessionValid(doc))
            {
                return new TatoebaResponse<SearchResults>
                {
                    Status = TatoebaStatus.InvalidSession,
                };
            }

            SearchResults results = new SearchResults();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<string> pages = new List<string>();

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//span[@class='pageNumber' or @class='current pageNumber']"))
            {
                var nav = node.CreateNavigator();
                string pageNumber = nav.Evaluate<string>("string(.)");

                if(!pages.Contains(pageNumber))
                {
                    pages.Add(pageNumber);
                }
            }

            results.PageCount = GetPageCount(pages);

            foreach (var nodeSet in doc.DocumentNode.SelectNodesOrEmpty("//*[@class='sentences_set']"))
            {
                string itemsPath = ".//*[@class='sentence mainSentence']|.//*[@class='translations']/*[@data-sentence-id]|.//div[@class='more']/*[@data-sentence-id]";

                SentenceSet sentenceSet = new SentenceSet();

                results.Results.Add(sentenceSet);

                foreach (var node in nodeSet.SelectNodesOrEmpty(itemsPath))
                {
                    var nav = node.CreateNavigator();

                    sentenceSet.Add(new Contribution
                    {
                        Text = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.TextPath),
                        Id = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.IdPath),
                        Language = new Language { Iso = nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.LanguagePath) },
                        Direction = (Direction)directions.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.DirectionPath)),
                        TranslationType = (TranslationType)translationTypes.IndexOf(nav.Evaluate<string>(XpathConfig.SentenceDetailConfig.TranslationConfig.TranslationTypePath))
                    });
                }
            }

            foreach (var item in results.Results.SelectMany(x=>x.Sentences))
            {
                item.Language = MainService.Languages.Where(x => x.Iso == item.Language.Iso).SingleOrDefault();
            }

            return new TatoebaResponse<SearchResults>
            {
                Content = results,
            };

        }
    }
}
