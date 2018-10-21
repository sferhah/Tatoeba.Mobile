using HtmlAgilityPack;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tatoeba.Mobile.Models;

namespace Tatoeba.Mobile.Services
{
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

        public static Contribution[] ParseContribs(string input)
        {
            HtmlDocument doc = LoadHtmlAndCheckSession(input);

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

            return sentences.ToArray();
        }


        public static SentenceDetail ParseSetenceDetail(string input)
        {
            HtmlDocument doc = LoadHtmlAndCheckSession(input);

            var docNav = doc.CreateNavigator();

            SentenceDetail setenceDetails = new SentenceDetail
            {
                IsEditable = docNav.Evaluate<bool>(XpathConfig.SentenceDetailConfig.TranslationConfig.IsEditablePath),
                PreviousId = docNav.Evaluate<string>(XpathConfig.SentenceDetailConfig.PreviousIdPath)?.Split('/').Last(),
                NextId = docNav.Evaluate<string>(XpathConfig.SentenceDetailConfig.NextIdPath)?.Split('/').Last(),
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

            return setenceDetails;            
        }
        
        public static SearchResults ParseSearchResults(string input)
        {
            HtmlDocument doc = LoadHtmlAndCheckSession(input);

            SearchResults results = new SearchResults();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<string> pages = new List<string>();

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.SearchResultsConfig.PageItemsPath))
            {
                var nav = node.CreateNavigator();
                string pageNumber = nav.Evaluate<string>(XpathConfig.SearchResultsConfig.PagePath);

                if(!pages.Contains(pageNumber))
                {
                    pages.Add(pageNumber);
                }
            }

            results.PageCount = GetPageCount(pages);

            foreach (var nodeSet in doc.DocumentNode.SelectNodesOrEmpty(XpathConfig.SearchResultsConfig.SentenceSetItemsPath))
            {
                SentenceSet sentenceSet = new SentenceSet();

                results.Results.Add(sentenceSet);

                foreach (var node in nodeSet.SelectNodesOrEmpty(XpathConfig.SearchResultsConfig.ItemsPath))
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

            return results;
        }

        private static int GetPageCount(List<string> input)
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

        private static HtmlDocument LoadHtmlAndCheckSession(string input)
        {
            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(input);

            if (!IsSessionValid(doc))
            {
                throw new InvalidSessionException();
            }

            return doc;
        }
    }
}
