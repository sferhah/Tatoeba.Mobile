using System;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Tatoeba.Mobile.Resx
{
    public class AppResources
    {
        private static readonly ResourceManager _DefaultResourceManager;

        static AppResources()
        {
            _DefaultResourceManager = ResourceManager;
        }

        private static ResourceManager _ResourceManager;

        internal static ResourceManager ResourceManager
        {
            get
            {
                if(_ResourceManager == null)
                {
                    SetIso("eng");
                }

                return _ResourceManager;
            }
            set => _ResourceManager = value;
        }

        public static void SetIso(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("iso can not be null or empty.");
            }

            string iso = value.ToLowerInvariant();

            var assembly = typeof(AppResources).GetTypeInfo().Assembly;
            var assembly_namespace = typeof(AppResources).Assembly.GetName().Name;            

            if (!assembly.GetManifestResourceNames().Any(x => x.Contains(assembly_namespace + $".Resx.AppResources.{iso}")))
            {
                iso = "eng";
            }

            ResourceManager = new ResourceManager(assembly_namespace + $".Resx.AppResources.{iso}", assembly);
        }

        internal static string GetString([CallerMemberName] string propertyName = null) 
            => ResourceManager.GetString(propertyName) ??
            _DefaultResourceManager.GetString(propertyName)
            ?? "$" + propertyName;

        public static string AddSentence => GetString();
        public static string AddTranslation => GetString();
        public static string Any => GetString();
        public static string Browse => GetString();
        public static string BrowseByLanguage => GetString();
        public static string Cancel => GetString();
        public static string ChangeLanguage => GetString();
        public static string Comments => GetString();
        public static string Edit => GetString();
        public static string EditSentence => GetString();
        public static string Error => GetString();
        public static string ErrorLoadingResources => GetString();
        public static string ErrorOccured => GetString();
        public static string ErrorParsingHtml => GetString();
        public static string FewerTranslations => GetString();
        public static string From => GetString();
        public static string HasAudio => GetString();
        public static string InvalidCredentials => GetString();
        public static string IsOrphan => GetString();
        public static string IsUnapproved => GetString();
        public static string LoadingResources => GetString();
        public static string LogOut => GetString();
        public static string Logs => GetString();
        public static string NewSentence => GetString();
        public static string NewTranslation => GetString();
        public static string Next => GetString();
        public static string No => GetString();
        public static string Ok => GetString();
        public static string PageInPagination => GetString();        
        public static string Password => GetString();
        public static string Previous => GetString();
        public static string Random => GetString();        
        public static string Recent => GetString();
        public static string Refresh => GetString();
        public static string Reload => GetString();
        public static string Search => GetString();
        public static string SearchResultsTitle => GetString();        
        public static string SearchTitle => GetString();
        public static string SentenceId => GetString();
        public static string ShowMoreTranslations => GetString();
        public static string Username => GetString();
        public static string To => GetString();
        public static string Translations => GetString();
        public static string Yes => GetString();
        public static string Validate => GetString();
    }
}
