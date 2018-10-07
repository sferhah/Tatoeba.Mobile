using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tatoeba.Mobile.Models;

namespace Tatoeba.Mobile.Services
{
    public static class HtmlAgilityPackExtensions
    {
        public static HtmlNodeCollection SelectNodesOrEmpty(this HtmlNode @this, string xpath)
            => @this.SelectNodes(xpath) ?? new HtmlNodeCollection(null);
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


            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//md-list-item"))
            {
                sentences.Add(new Contribution
                {
                    Text = HttpUtility.HtmlDecode(node.SelectSingleNode("div/div").InnerText).Trim(),
                    Direction = (Direction)directions.IndexOf(node.SelectSingleNode("div/div").Attributes["dir"].Value),
                    DateText = HttpUtility.HtmlDecode(node.SelectSingleNode("div/p").InnerText).Trim(),
                    Id = node.SelectSingleNode("md-button").Attributes["href"].Value.Split('/').Last(),
                    Language = new Language { Iso = node.SelectSingleNode("img").Attributes["alt"].Value },
                    ContribType = (ContribType)contribTypes.IndexOf(node.Attributes["class"].Value),
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

            SentenceDetail setenceDetails = new SentenceDetail();
            setenceDetails.IsEditable = doc.DocumentNode.SelectSingleNode("//*[@class=\"sentence mainSentence\"]//*[@class=\"text correctnessZero editableSentence\"]") != null;


            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//*[@class=\"sentence mainSentence\"]|//*[@class=\"translations\"]/*[@data-sentence-id]|//div[@class=\"more\"]/*[@data-sentence-id]"))
            {
                string text = HttpUtility.HtmlDecode(node.SelectSingleNode(".//*[@class=\"text correctnessZero\"]|.//*[@class=\"text correctnessZero editableSentence\"]").InnerText).Trim();
                string language = node.SelectSingleNode(".//img").Attributes["alt"].Value;
                string sentenceId = node.Attributes["data-sentence-id"].Value;
                Direction direction = (Direction)directions.IndexOf(node.SelectSingleNode(".//*[@class=\"text correctnessZero\"]|//*[@class=\"text correctnessZero editableSentence\"]").Attributes["dir"].Value);
                var navigationIcon = node.SelectSingleNode("div/a").Attributes["class"].Value;

                setenceDetails.Sentences.Add(new Contribution
                {
                    Text = text,
                    Id = sentenceId,
                    Language = new Language { Iso = language },
                    Direction = direction,
                    TranslationType = (TranslationType)translationTypes.IndexOf(navigationIcon)
                });
            }           

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//md-list-item"))
            {
                setenceDetails.Logs.Add(new Log
                {
                    Text = HttpUtility.HtmlDecode(node.SelectSingleNode("div/div").InnerText).Trim(),
                    Direction = (Direction)directions.IndexOf(node.SelectSingleNode("div/div").Attributes["dir"].Value),
                    DateText = HttpUtility.HtmlDecode(node.SelectSingleNode("div/p").InnerText).Trim(),
                    ContribType = (ContribType)contribTypes.IndexOf(node.Attributes["class"].Value),
                });
            }

            foreach (var node in doc.DocumentNode.SelectNodesOrEmpty("//md-card"))
            {
                setenceDetails.Comments.Add(new Comment
                {
                    Username = HttpUtility.HtmlDecode(node.SelectSingleNode(".//*[@class=\"md-title\"]").InnerText).Trim(),
                    Content = HttpUtility.HtmlDecode(node.SelectSingleNode(".//md-card-content").InnerText).Trim(),
                    DateText = HttpUtility.HtmlDecode(node.SelectSingleNode(".//*[@class=\"md-subhead ellipsis\"]").InnerText).Trim(),
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
