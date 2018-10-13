using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Storage;

namespace Tatoeba.Mobile.Services
{
    public class MainService
    {

        const string flags_url = "https://raw.githubusercontent.com/Tatoeba/tatoeba2/dev/app/webroot/img/flags/";
        const string languages_url = "https://raw.githubusercontent.com/Tatoeba/tatoeba2/dev/app/Lib/LanguagesLib.php";
        const string latest_contribs = "https://tatoeba.org/eng/contributions/latest/";
        const string search_url = "https://tatoeba.org/eng/sentences/search?";
        const string sentence_url = " https://tatoeba.org/eng/sentences/show/";
        const string url_save_translation = "https://tatoeba.org/eng/sentences/save_translation";
        const string url_edit_sentence = "https://tatoeba.org/eng/sentences/edit_sentence";
        const string url_login = "https://tatoeba.org/eng/users/check_login?redirectTo=%2Feng";
        const string url_main = "https://tatoeba.org/eng/";

        const string cookies_file_name = "cookies.ck";

        static HttpTatotebaClient client = new HttpTatotebaClient();

        public static List<Language> Languages { get; set; }

        public static async Task<bool> InitAsync()
        {
            await AppDbContext.InitAsync().ConfigureAwait(false);

            using (AppDbContext context = new AppDbContext())
            {
                Languages = await context.Languages.OrderBy(x => x.Label).ToListAsync().ConfigureAwait(false);

                if (Languages.Count() == 0)
                {
                    Languages = await GetLanguages().ConfigureAwait(false);
                    await context.Languages.AddRangeAsync(Languages).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            Languages.Insert(0, new Language { Flag = null, Iso = null, Label = "All languages" });
            var existence = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name).ConfigureAwait(false);
            var exists = existence == PCLStorage.ExistenceCheckResult.FileExists;


            if (exists)
            {
                client.cookies = await ReadCookiesFromDisk(cookies_file_name).ConfigureAwait(false);
            }

            return exists;
        }


        public static async Task ClearCookiers()
        {
            client.cookies = new CookieContainer();
            var exists = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name).ConfigureAwait(false) == PCLStorage.ExistenceCheckResult.FileExists;            

            if(!exists)
            {
                return;
            }

            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(cookies_file_name).ConfigureAwait(false);
            await file.DeleteAsync().ConfigureAwait(false);
        }

      

        public static async Task<List<Language>> GetLanguages()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetStringAsync(languages_url).ConfigureAwait(false);

            var languages = response.Substring("$languages = array(", ");").Split('\n')
                .Where(x => x.Contains("__d('languages',")).Select(x => x.Trim().ToLanguage())
                .Where(y => y != null)
                .ToList();


            async Task DownloadFlag(Language lan) => lan.Flag = await client.GetByteArrayAsync(flags_url + lan.Iso + ".png").ConfigureAwait(false);

            await Task.WhenAll(languages.Select(x => DownloadFlag(x)).ToArray()).ConfigureAwait(false);

            return languages;
        }

        public static async Task<TatoebaResponse<string>> SearchAsync(string text,
            string isoFrom, 
            string isoTo,
            bool? isOrphan = false,
            bool? isUnapproved = false,
            bool? hasAudio = null)
        {
            string orphans = isOrphan == null ? string.Empty : (isOrphan.Value ? "yes" : "no");
            string unapproved = isUnapproved == null ? string.Empty : (isUnapproved.Value ? "yes" : "no");
            string has_audio = hasAudio == null ? string.Empty : (hasAudio.Value ? "yes" : "no");

            string url = search_url + $"from={isoFrom?? "und"}" +
                $"&to={isoTo ?? "und"}" +
                $"&query={text.UrlEncode()}" +
                $"&orphans={orphans}" +
                $"&unapproved={unapproved}" +
                $"&has_audio={has_audio}";

            var response = await client.GetStringAsync(url).ConfigureAwait(false);

            return new TatoebaResponse<string>
            {
                Content = response,
            };
        }

        public static async Task<TatoebaResponse<Contribution[]>> GetLatestContributions(string language)
        {
            var response = await client.GetStringAsync(latest_contribs + language).ConfigureAwait(false);
            return TatoebaScraper.ParseContribs(response);
        }


        public static async Task<TatoebaResponse<SentenceDetail>> GetSentenceDetail(string id)
        {
            var result = await client.GetStringAsync(sentence_url + id).ConfigureAwait(false);
            var response = TatoebaScraper.ParseSetenceDetail(result);

            if(response.Content != null)
            {
                response.Content.Id = id;
            }
            
            return response;
        }



        public static async Task<string> SaveTranslation(string originalSentenceId, SentenceBase sentence)
        {
            string postData = "id=" + originalSentenceId.UrlEncode()
             + "&" + "selectLang" + "=" + sentence.Language.Iso.UrlEncode()
             + "&" + "value" + "=" + sentence.Text.UrlEncode();

            string respStr = await client.PostAsync(url_save_translation, postData).ConfigureAwait(false);


            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(respStr);

            return doc.CreateNavigator().Evaluate<string>("string(//*[@class=\"sentenceContent\"]/@data-sentence-id)");
        }


        public static async Task EditSentence(SentenceBase sentence)
        {
            string postData = "value" + "=" + sentence.Text.UrlEncode()
                + "&" + "id=" + sentence.Language.Iso + "_" + sentence.Id.UrlEncode();

            await client.PostAsync(url_edit_sentence, postData).ConfigureAwait(false);
        }    

        /// <summary>Logs in and retrieves cookies.</summary>
        public static async Task<bool> LogInAsync(string userName, string userPass)
        {
            var result = await client.GetStringAsync(url_main).ConfigureAwait(false);

            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);

            var key_value = doc.CreateNavigator().Evaluate<string>(XpathLoginConfig.KeyPath);

            if (string.IsNullOrWhiteSpace(key_value)) // already logged in
            {
                return true;
            }
      
            var fields_value = doc.CreateNavigator().Evaluate<string>(XpathLoginConfig.ValuePath);

            string postData = "_method=POST"
                + "&" + "data[_Token][key]".UrlEncode() + "=" + key_value
                + "&" + "data[User][username]".UrlEncode() + "=" + userName.UrlEncode()
                + "&" + "data[User][password]".UrlEncode() + "=" + userPass.UrlEncode()
                + "&" + "data[User][rememberMe]".UrlEncode() + "=" + "1"
                + "&" + "data[_Token][fields]".UrlEncode() + "=" + fields_value
                + "&" + "data[_Token][unlocked]".UrlEncode() + "=" + "";


            string respStr = await client.PostAndSaveCookiesAsync(url_login, postData, true).ConfigureAwait(false);

            HtmlDocument responseDoc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            responseDoc.LoadHtml(respStr);            

            var success = responseDoc.CreateNavigator().Evaluate<bool>(XpathLoginConfig.SuccessPath);

            if (success)
            {
                await WriteCookiesToDisk(cookies_file_name, client.cookies).ConfigureAwait(false);
            }

            return success;
        }

        public static async Task WriteCookiesToDisk(string fileName, CookieContainer cookieJar)
        {
            try
            {
                var file = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync(fileName, PCLStorage.CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);
                using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, cookieJar);             
                }
            }
            catch (Exception e)
            {
            }
        }

        public static async Task<CookieContainer> ReadCookiesFromDisk(string fileName)
        {
            try
            {
               var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(fileName).ConfigureAwait(false);                

                using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite).ConfigureAwait(false))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (CookieContainer)formatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                return new CookieContainer();
            }
        }

    }
}
