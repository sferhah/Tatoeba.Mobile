using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Storage;

namespace Tatoeba.Mobile.Services
{
    public class CacheUtils
    {
        public static readonly int Version = 1;
        public static readonly string TatoebaConfigFileName = $"TatoebaConfig_v{Version}.json";
        
        public static Stream GetCacheStream(string cacheFile)
        {
            var assembly = typeof(CacheUtils).GetTypeInfo().Assembly;
            var assembly_namespace = typeof(CacheUtils).Assembly.GetName().Name;

            return assembly.GetManifestResourceStream(assembly_namespace + ".Cache." + cacheFile);
        }
    }

    public class MainService
    {
        const string cookies_file_name = "cookies.ck";

        static HttpTatotebaClient client = new HttpTatotebaClient();

        public static List<Language> Languages { get; set; }

        public static TatoebaConfig TatoebaConfig { get; set; }

        public static async Task<bool> InitAsync()
        {

            //var json = JsonConvert.SerializeObject(new TatoebaConfig(), new JsonSerializerSettings
            //{
            //    Formatting = Formatting.Indented,
            //});

            TatoebaConfig = await GetTatoebaConfig().ConfigureAwait(false);
            TatoebaScraper.XpathConfig = TatoebaConfig.XpathConfig;
            
            //using (Stream stream = CacheUtils.GetCacheStream(CacheUtils.TatoebaConfigFileName))
            //{
            //    StreamReader reader = new StreamReader(stream);
            //    string text = await reader.ReadToEndAsync().ConfigureAwait(false);

            //    TatoebaConfig = JsonConvert.DeserializeObject<TatoebaConfig>(text);
            //    TatoebaScraper.XpathConfig = TatoebaConfig.XpathConfig;
            //}

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

        public static async Task<TatoebaConfig> GetTatoebaConfig()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetStringAsync("https://raw.githubusercontent.com/sferhah/Tatoeba.Mobile/master/Tatoeba.Mobile/Tatoeba.Mobile/Cache/TatoebaConfig_v1.json").ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TatoebaConfig>(response);
        }


        public static async Task<List<Language>> GetLanguages()
        {
            HttpClient client = new HttpClient();

            var response = await client.GetStringAsync(TatoebaConfig.UrlConfig.Languages).ConfigureAwait(false);

            var languages = response.Substring("$languages = array(", ");").Split('\n')
                .Where(x => x.Contains("__d('languages',")).Select(x => x.Trim().ToLanguage())
                .Where(y => y != null)
                .ToList();


            async Task DownloadFlag(Language lan) => lan.Flag = await client.GetByteArrayAsync(TatoebaConfig.UrlConfig.Flags + lan.Iso + ".png").ConfigureAwait(false);

            await Task.WhenAll(languages.Select(x => DownloadFlag(x)).ToArray()).ConfigureAwait(false);

            return languages;
        }

        public static async Task<TatoebaResponse<List<SentenceDetail>>> SearchAsync(string text,
            string isoFrom, 
            string isoTo,
            bool? isOrphan = false,
            bool? isUnapproved = false,
            bool? hasAudio = null,
            bool? isTransOrphan = false,
            bool? isTransUnapproved = false,
            bool? TransHasAudio = null)
        {
            string orphans = isOrphan == null ? string.Empty : (isOrphan.Value ? "yes" : "no");
            string unapproved = isUnapproved == null ? string.Empty : (isUnapproved.Value ? "yes" : "no");
            string has_audio = hasAudio == null ? string.Empty : (hasAudio.Value ? "yes" : "no");

            string trans_orphan = isTransOrphan == null ? string.Empty : (isTransOrphan.Value ? "yes" : "no");
            string trans_unapproved = isTransUnapproved == null ? string.Empty : (isTransUnapproved.Value ? "yes" : "no");
            string trans_has_audio = TransHasAudio == null ? string.Empty : (TransHasAudio.Value ? "yes" : "no");

            string url = TatoebaConfig.UrlConfig.Search + $"from={isoFrom?? "und"}" +
                $"&to={isoTo ?? "und"}" +
                $"&query={text.UrlEncode()}" +
                $"&orphans={orphans}" +
                $"&unapproved={unapproved}" +
                $"&has_audio={has_audio}" +
                $"&trans_orphan={trans_orphan}" +
                $"&trans_unapproved={trans_unapproved}" +
                $"&trans_has_audio={trans_has_audio}";

            var response = await client.GetStringAsync(url).ConfigureAwait(false);

            return TatoebaScraper.ParseSearchResults(response);            
        }

        public static async Task<TatoebaResponse<Contribution[]>> GetLatestContributions(string language)
        {
            var response = await client.GetStringAsync(TatoebaConfig.UrlConfig.LatestContribs + language).ConfigureAwait(false);
            return TatoebaScraper.ParseContribs(response);
        }


        public static async Task<TatoebaResponse<SentenceDetail>> GetSentenceDetail(string id)
        {
            var result = await client.GetStringAsync(TatoebaConfig.UrlConfig.Sentence + id).ConfigureAwait(false);
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

            string respStr = await client.PostAsync(TatoebaConfig.UrlConfig.SaveTranslation, postData).ConfigureAwait(false);


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

            await client.PostAsync(TatoebaConfig.UrlConfig.EditSentence, postData).ConfigureAwait(false);
        }

        static XpathLoginConfig XpathLoginConfig => TatoebaScraper.XpathConfig.LoginConfig;

        /// <summary>Logs in and retrieves cookies.</summary>
        public static async Task<bool> LogInAsync(string userName, string userPass)
        {
            var result = await client.GetStringAsync(TatoebaConfig.UrlConfig.Main).ConfigureAwait(false);

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


            string respStr = await client.PostAndSaveCookiesAsync(TatoebaConfig.UrlConfig.Login, postData, true).ConfigureAwait(false);

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
