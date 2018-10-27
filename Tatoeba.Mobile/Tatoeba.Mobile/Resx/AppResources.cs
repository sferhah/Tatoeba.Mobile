using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Tatoeba.Mobile.Resx
{
    public class AppResources
    {
        private static ResourceManager _ResourceManager;
        public static ResourceManager ResourceManager
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

            ResourceManager = new ResourceManager(assembly_namespace + $".Resx.AppResources.{iso}", assembly);

            try
            {
                ResourceManager.GetString(nameof(ErrorLoadingResources));
            }
            catch (MissingManifestResourceException)
            {
                iso = "eng";
                ResourceManager = new ResourceManager(assembly_namespace + $".Resx.AppResources.{iso}", assembly);
            }
        }

        internal static string GetString([CallerMemberName] string propertyName = null) => ResourceManager.GetString(propertyName);

        public static string Cancel => GetString();
        public static string Error => GetString();
        public static string ErrorLoadingResources => GetString();
        public static string LoadingResources => GetString();
        public static string Ok => GetString();
        public static string Password => GetString();
        public static string Refresh => GetString();
        public static string Reload => GetString();
        public static string Username => GetString();
        public static string Validate => GetString();
    }
}
