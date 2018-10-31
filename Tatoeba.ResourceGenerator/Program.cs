using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Tatoeba.Mobile.Services;

namespace Tatoeba.ResourceGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Xamarin.Forms.Mocks.MockForms.Init();        

            var languages = await MainService.GetLanguages();

            var languageListHash = await SerializeToFile(languages, "Languages.json");
            await SerializeToFile(new TatoebaConfig { LanguageListHash = languageListHash }, "TatoebaConfig_v2.json");            
        }

        static async Task<string> SerializeToFile(object input, string fileName)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            while (!path.EndsWith("Tatoeba.ResourceGenerator"))
            {
                path = System.IO.Directory.GetParent(path).FullName;
            }

            var json = JsonConvert.SerializeObject(input, new JsonSerializerSettings { Formatting = Formatting.Indented, });
            await File.WriteAllTextAsync(Path.Combine(path, fileName), json);

            return MainService.GetHashString(json);
        }
    }
}
