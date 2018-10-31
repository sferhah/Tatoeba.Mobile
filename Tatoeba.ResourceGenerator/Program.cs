using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tatoeba.Mobile.Models;
using Tatoeba.Mobile.Services;

namespace Tatoeba.ResourceGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Xamarin.Forms.Mocks.MockForms.Init();

            var languages = await GetLanguages();

            var languageListHash = await SerializeToCacheFile(languages, CacheUtils.LanguageListFile);           

            await SerializeToCacheFile(new TatoebaConfig { LanguageListHash = languageListHash }, CacheUtils.TatoebaConfigFileName);
        }

        public static async Task<List<Language>> GetLanguages()
        {
            HttpTatotebaClient cookieLessClient = new HttpTatotebaClient(60);

            var response = await cookieLessClient.GetAsync<string>(new TatoebaConfig().UrlConfig.Languages).ConfigureAwait(false);

            if (response.Error != null)
            {
                throw new Exception(response.Error);
            }

            var languages = response.Content.Substring("$languages = array(", ");").Split('\n')
                .Where(x => x.Contains("__d('languages',")).Select(x => x.Trim().ToLanguage())
                .Where(y => y != null)
                .ToList();

            async Task DownloadFlag(Language lan) => lan.Flag = await cookieLessClient.GetByteArrayAsync(new TatoebaConfig().UrlConfig.Flags + lan.Iso + ".png").ConfigureAwait(false);

            await Task.WhenAll(languages.Select(x => DownloadFlag(x)).ToArray()).ConfigureAwait(false);

            return languages;
        }

        static async Task<string> SerializeToCacheFile(object input, string fileName)
        {
            string path = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;

            while (!System.IO.Directory.GetFiles(path).Any(x => x.EndsWith(".sln")))
            {
                path = System.IO.Directory.GetParent(path).FullName;
            }

            path = Path.Combine(path, @"Tatoeba.Mobile\Tatoeba.Mobile\Cache", fileName);

            var json = JsonConvert.SerializeObject(input, new JsonSerializerSettings { Formatting = Formatting.Indented, });
            await File.WriteAllTextAsync(path, json);

            return GetHashString(json);
        }

        public static string GetHashString(string inputString)
        {
            var data = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));

            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public static class Xt
    {
        public static Language ToLanguage(this string @this)
        {
            var matches = new Regex("'.*?'").Matches(@this).Cast<Match>().Select(x => x.ToString()).Select(mystr => mystr.Substring(1, mystr.Length - 2)).ToList();

            return matches.Count != 3 ? null : new Language
            {
                Iso = matches[0],
                Label = matches[2],
            };
        }
    }
}
