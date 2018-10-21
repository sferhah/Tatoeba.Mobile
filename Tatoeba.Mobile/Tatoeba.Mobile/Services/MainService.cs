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

        public static List<Language> IsoLanguages { get; set; }
        public static List<Language> Languages { get; set; }
        public static List<Language> BrowsableLanguages { get; set; }
        public static List<Language> TransBrowsableLanguages { get; set; }

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

            using (AppDbContext context = new AppDbContext())
            {
                await context.Database.EnsureCreatedAsync().ConfigureAwait(false);

                IsoLanguages = await context.Languages.OrderBy(x => x.Label).ToListAsync().ConfigureAwait(false);

                if (IsoLanguages.Count() == 0)
                {
                    IsoLanguages = await GetLanguages().ConfigureAwait(false);
                    await context.Languages.AddRangeAsync(IsoLanguages).ConfigureAwait(false);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
            }

            Languages = IsoLanguages.ToList();
            Languages.Insert(0, new Language { Flag = null, Iso = null, Label = "All languages" });

            BrowsableLanguages = IsoLanguages.ToList();
            BrowsableLanguages.Add(new Language { Flag = null, Iso = "unknown", Label = "Unknown language" });

            TransBrowsableLanguages = IsoLanguages.ToList();
            TransBrowsableLanguages.Insert(0, new Language { Flag = null, Iso = "und", Label = "All languages" });
            TransBrowsableLanguages.Insert(0, new Language { Flag = null, Iso = "none", Label = "None" });



            var existence = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name).ConfigureAwait(false);
            var exists = existence == PCLStorage.ExistenceCheckResult.FileExists;

            if (exists)
            {
                client.Cookies = await ReadCookiesFromDisk(cookies_file_name).ConfigureAwait(false);
            }

            return exists;
        }


        public static async Task ClearCookiers()
        {
            client.Cookies = new CookieContainer();
            var exists = await PCLStorage.FileSystem.Current.LocalStorage.CheckExistsAsync(cookies_file_name).ConfigureAwait(false) == PCLStorage.ExistenceCheckResult.FileExists;

            if (!exists)
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


        public static async Task<TatoebaResponse<SearchResults>> GetSentencesAsync(SearchRequest request)
            => request.Mode == QueryMode.Search ? await SearchAsync(request) : await BrowseAsync(request);

        public static async Task<TatoebaResponse<SearchResults>> SearchAsync(SearchRequest request)
        {
            string orphans = request.IsOrphan == null ? string.Empty : (request.IsOrphan.Value ? "yes" : "no");
            string unapproved = request.IsUnapproved == null ? string.Empty : (request.IsUnapproved.Value ? "yes" : "no");
            string has_audio = request.HasAudio == null ? string.Empty : (request.HasAudio.Value ? "yes" : "no");

            string trans_orphan = request.IsTransOrphan == null ? string.Empty : (request.IsTransOrphan.Value ? "yes" : "no");
            string trans_unapproved = request.IsTransUnapproved == null ? string.Empty : (request.IsTransUnapproved.Value ? "yes" : "no");
            string trans_has_audio = request.TransHasAudio == null ? string.Empty : (request.TransHasAudio.Value ? "yes" : "no");

            string url = TatoebaConfig.UrlConfig.Search + $"page:{request.Page}?" +
                $"from={request.IsoFrom ?? "und"}" +
                $"&to={request.IsoTo ?? "und"}" +
                $"&query={request.Text.UrlEncode()}" +
                $"&orphans={orphans}" +
                $"&unapproved={unapproved}" +
                $"&has_audio={has_audio}" +
                $"&trans_orphan={trans_orphan}" +
                $"&trans_unapproved={trans_unapproved}" +
                $"&trans_has_audio={trans_has_audio}";

            var response = await client.GetAsync<SearchResults>(url).ConfigureAwait(false);

            if (response.Content != null)
            {
                response.Content.Request = request;                
            }

            return response;
        }

        public static async Task<TatoebaResponse<SearchResults>> BrowseAsync(SearchRequest request)
        {   
            string url = TatoebaConfig.UrlConfig.Browse + 
                $"{request.IsoFrom ?? "und"}/{request.IsoTo ?? "none"}/indifferent/page:{request.Page}";

            var response = await client.GetAsync<SearchResults>(url).ConfigureAwait(false);

            if (response.Content != null)
            {
                response.Content.Request = request;
            }

            return response;
        }


        public static async Task<TatoebaResponse<Contribution[]>> GetLatestContributions(string language) 
            => await client.GetAsync<Contribution[]>(TatoebaConfig.UrlConfig.LatestContribs + language).ConfigureAwait(false);


        public static async Task<TatoebaResponse<SentenceDetail>> GetSentenceDetail(string id)
        {
            var result = await client.GetAsync<SentenceDetail>(TatoebaConfig.UrlConfig.Sentence + id).ConfigureAwait(false);            

            if(result.Content != null)
            {
                result.Content.Id = id;
            }
            
            return result;
        }



        public static async Task<string> SaveTranslation(string originalSentenceId, SentenceBase sentence)
        {
            string postData = "id=" + originalSentenceId.UrlEncode()
             + "&" + "selectLang" + "=" + sentence.Language.Iso.UrlEncode()
             + "&" + "value" + "=" + sentence.Text.UrlEncode();

            var resp = await client.PostAsync<string>(TatoebaConfig.UrlConfig.SaveTranslation, postData).ConfigureAwait(false);


            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(resp.Content);

            return doc.CreateNavigator().Evaluate<string>("string(//*[@class=\"sentenceContent\"]/@data-sentence-id)");
        }

        public static async Task AddNewSentence(SentenceBase sentence)
        {
            string postData = "value" + "=" + sentence.Text.UrlEncode()
             + "&" + "selectedLang" + "=" + sentence.Language.Iso.UrlEncode();

            await client.PostAsync<string>(TatoebaConfig.UrlConfig.NewSentence, postData).ConfigureAwait(false);
        }

        public static async Task EditSentence(SentenceBase sentence)
        {
            string postData = "value" + "=" + sentence.Text.UrlEncode()
                + "&" + "id=" + sentence.Language.Iso + "_" + sentence.Id.UrlEncode();

            await client.PostAsync<string>(TatoebaConfig.UrlConfig.EditSentence, postData).ConfigureAwait(false);
        }

        static XpathLoginConfig XpathLoginConfig => TatoebaScraper.XpathConfig.LoginConfig;

        public static async Task<TatoebaResponse<bool>> LogInAsync(string userName, string userPass)
        {
            var result = await client.GetAsync<string>(TatoebaConfig.UrlConfig.Main).ConfigureAwait(false);

            if(result.Status != TatoebaStatus.Success)
            {
                return new TatoebaResponse<bool>
                {
                    Content = false,
                    Status = result.Status,
                };
            }

            HtmlDocument doc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            doc.LoadHtml(result.Content);

            var key_value = doc.CreateNavigator().Evaluate<string>(XpathLoginConfig.KeyPath);

            if (string.IsNullOrWhiteSpace(key_value)) // already logged in
            {
                return new TatoebaResponse<bool>
                {
                    Content = true,
                    Status = TatoebaStatus.Success,
                };
            }
      
            var fields_value = doc.CreateNavigator().Evaluate<string>(XpathLoginConfig.ValuePath);

            string postData = "_method=POST"
                + "&" + "data[_Token][key]".UrlEncode() + "=" + key_value
                + "&" + "data[User][username]".UrlEncode() + "=" + userName.UrlEncode()
                + "&" + "data[User][password]".UrlEncode() + "=" + userPass.UrlEncode()
                + "&" + "data[User][rememberMe]".UrlEncode() + "=" + "1"
                + "&" + "data[_Token][fields]".UrlEncode() + "=" + fields_value
                + "&" + "data[_Token][unlocked]".UrlEncode() + "=" + "";


            var resp = await client.PostAsync<string>(TatoebaConfig.UrlConfig.Login, postData).ConfigureAwait(false);

            HtmlDocument responseDoc = new HtmlDocument
            {
                OptionFixNestedTags = true
            };

            responseDoc.LoadHtml(resp.Content);            

            var success = responseDoc.CreateNavigator().Evaluate<bool>(XpathLoginConfig.SuccessPath);

            if (success)
            {
                await WriteCookiesToDisk(cookies_file_name, client.Cookies).ConfigureAwait(false);
            }

            return new TatoebaResponse<bool>
            {
                Content = success,
                Status = TatoebaStatus.Success,
            };
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
