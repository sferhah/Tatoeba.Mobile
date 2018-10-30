using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Tatoeba.Mobile.PlatformSpecific
{  
    public interface ILocalize
    {
        string ThreeLetterISOLanguageName { get; }
    }

    public static class Localize
    {
        private static Dictionary<string,string> Iso639_2_ToIso639_3_Map;
        private static Dictionary<string,string> MacroLanguageToStandardMap;

        static Localize()
        {
            var assembly = typeof(ILocalize).GetTypeInfo().Assembly;
            var assembly_namespace = typeof(ILocalize).Assembly.GetName().Name;

            var names = assembly.GetManifestResourceNames();

            Iso639_2_ToIso639_3_Map = ReadLines(() => assembly.GetManifestResourceStream(assembly_namespace + ".PlatformSpecific.iso-639-2-to-iso-639-3.tab"))
                .Skip(1).Select(x => x.Split('\t')).ToDictionary(arr => arr[0], arr => arr[1]);

            MacroLanguageToStandardMap = ReadLines(() => assembly.GetManifestResourceStream(assembly_namespace + ".PlatformSpecific.macrolanguage-to-standard.tab"))
                .Skip(1).Select(x => x.Split('\t')).ToDictionary(arr => arr[0], arr => arr[1]);
        }

        public static IEnumerable<string> ReadLines(Func<Stream> streamProvider)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
       
        public static string ThreeLetterISOLanguageName
        {
            get
            {
                var iso639_2 = DependencyService.Get<ILocalize>().ThreeLetterISOLanguageName;

                string iso639_3;

                if (Iso639_2_ToIso639_3_Map.ContainsKey(iso639_2))
                {
                    iso639_3 = Iso639_2_ToIso639_3_Map[iso639_2];
                }
                else
                {
                    iso639_3 = iso639_2;
                }

                if (MacroLanguageToStandardMap.ContainsKey(iso639_3))
                {
                    iso639_3 = MacroLanguageToStandardMap[iso639_3];
                }

                return iso639_3;
            }
        }
    }
}

