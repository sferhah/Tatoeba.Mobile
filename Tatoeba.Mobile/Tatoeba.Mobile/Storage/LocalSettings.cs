using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Tatoeba.Mobile.Storage
{
    public class LocalSettings
    {
        private static ISettings AppSettings => CrossSettings.Current;

        public static string LastIsoSelection
        {
            get => AppSettings.GetValueOrDefault(nameof(LastIsoSelection), null);
            set => AppSettings.AddOrUpdateValue(nameof(LastIsoSelection), value);
        }

        public static string LastIsoTranslation
        {
            get => AppSettings.GetValueOrDefault(nameof(LastIsoTranslation), null);
            set => AppSettings.AddOrUpdateValue(nameof(LastIsoTranslation), value);
        }
      
    }
}
