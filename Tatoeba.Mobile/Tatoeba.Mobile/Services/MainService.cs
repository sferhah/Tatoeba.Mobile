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
        const string sentence_url = " https://tatoeba.org/eng/sentences/show/";
        const string cookies_file_name = "cookies.ck";

        static HttpTatotebaClient client = new HttpTatotebaClient();

        public static List<Language> Languages { get; set; }

        public static async Task<bool> InitAsync()
        {
            await AppDbContext.InitAsync();

            using (AppDbContext context = new AppDbContext())
            {
                Languages = await context.Languages.OrderBy(x => x.Label).ToListAsync();

                if (Languages.Count() == 0)
                {
                    Languages = await GetLanguages();
                    await context.Languages.AddRangeAsync(Languages);
                    await context.SaveChangesAsync();
                }
            }

            Languages.Insert(0, new Language { Flag = null, Iso = null, Label = "All languages" });
            var existence = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name);
            var exists = existence == PCLStorage.ExistenceCheckResult.FileExists;


            if (exists)
            {
                client.cookies = await ReadCookiesFromDisk(cookies_file_name);
            }

            return exists;
        }


        public static async Task ClearCookiers()
        {
            var exists = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name) == PCLStorage.ExistenceCheckResult.FileExists;            

            if(!exists)
            {
                return;
            }

            var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(cookies_file_name);
            await file.DeleteAsync();
        }

        public static async Task<List<Language>> GetLanguages()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetStringAsync(languages_url);

            var languages = response.Substring("$languages = array(", ");").Split('\n')
                .Where(x => x.Contains("__d('languages',")).Select(x => x.Trim().ToLanguage())
                .Where(y => y != null)
                .ToList();


            async Task DownloadFlag(Language lan) => lan.Flag = await client.GetByteArrayAsync(flags_url + lan.Iso + ".png");

            await Task.WhenAll(languages.Select(x => DownloadFlag(x)).ToArray());

            return languages;
        }


        public static async Task<TatoebaResponse<Contribution[]>> GetLatestContributions(string language)
        {
            var response = await client.GetStringAsync(latest_contribs + language);
            return TatoebaScraper.ParseContribs(response);
        }


        public static async Task<TatoebaResponse<SentenceDetail>> GetSentenceDetail(string id)
        {
            var result = await client.GetStringAsync(sentence_url + id);
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

            string respStr = await client.PostAsync("https://tatoeba.org/eng/sentences/save_translation", postData);


            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(respStr);


            var node = doc.DocumentNode.SelectNodesOrEmpty("//*[@class=\"sentenceContent\"]").FirstOrDefault();

            if (node == null)
            {
                return null;
            }

            return node.Attributes["data-sentence-id"].Value;
        }


        public static async Task EditSentence(SentenceBase sentence)
        {
            string postData = "value" + "=" + sentence.Text.UrlEncode()
                + "&" + "id=" + sentence.Language.Iso + "_" + sentence.Id.UrlEncode();

            await client.PostAsync("https://tatoeba.org/eng/sentences/edit_sentence", postData);
        }

        /// <summary>Logs in and retrieves cookies.</summary>
        public static async Task<bool> LogInAsync(string userName, string userPass)
        {
            var result = await client.GetStringAsync("https://tatoeba.org/eng/");

            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result);


            if(doc.DocumentNode.SelectNodes("//*[@name=\"data[_Token][key]\"]") == null) // already logged in
            {
                return true;
            }

            var key_value = doc.DocumentNode.SelectNodes("//*[@name=\"data[_Token][key]\"]").FirstOrDefault().Attributes["value"].Value;
            var fields_value = doc.DocumentNode.SelectNodes("//*[@name=\"data[_Token][fields]\"]").FirstOrDefault().Attributes["value"].Value;


            string postData = "_method=POST"
                + "&" + "data[_Token][key]".UrlEncode() + "=" + key_value
                + "&" + "data[User][username]".UrlEncode() + "=" + userName.UrlEncode()
                + "&" + "data[User][password]".UrlEncode() + "=" + userPass.UrlEncode()
                + "&" + "data[User][rememberMe]".UrlEncode() + "=" + "1"
                + "&" + "data[_Token][fields]".UrlEncode() + "=" + fields_value
                + "&" + "data[_Token][unlocked]".UrlEncode() + "=" + "";


            string respStr = await client.PostAndSaveCookiesAsync("https://tatoeba.org/eng/users/check_login?redirectTo=%2Feng", postData, true);

            var success = respStr.Contains("li id=\"profile\"");

            if(success)
            {
                await WriteCookiesToDisk(cookies_file_name, client.cookies);
            }

            return success;
        }

        public static async Task WriteCookiesToDisk(string fileName, CookieContainer cookieJar)
        {
            try
            {
                var file = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync(fileName, PCLStorage.CreationCollisionOption.ReplaceExisting);
                using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
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
               var file = await PCLStorage.FileSystem.Current.LocalStorage.GetFileAsync(fileName);                

                using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
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
