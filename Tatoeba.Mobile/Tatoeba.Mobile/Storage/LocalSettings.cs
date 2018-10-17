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


        public static string LastIsoSearchFrom
        {
            get => AppSettings.GetValueOrDefault(nameof(LastIsoSearchFrom), null);
            set => AppSettings.AddOrUpdateValue(nameof(LastIsoSearchFrom), value);
        }

        public static string LastIsoSearchTo
        {
            get => AppSettings.GetValueOrDefault(nameof(LastIsoSearchTo), null);
            set => AppSettings.AddOrUpdateValue(nameof(LastIsoSearchTo), value);
        }

        public static string LastRandomIso
        {
            get => AppSettings.GetValueOrDefault(nameof(LastRandomIso), null);
            set => AppSettings.AddOrUpdateValue(nameof(LastRandomIso), value);
        }

    }
}
