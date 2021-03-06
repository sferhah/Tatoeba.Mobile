﻿using Xamarin.Forms;

[assembly: Dependency(typeof(Tatoeba.Mobile.Droid.PlatformSpecific.Localize))]
namespace Tatoeba.Mobile.Droid.PlatformSpecific
{
    public class Localize : Mobile.PlatformSpecific.ILocalize
    {
        public string ThreeLetterISOLanguageName => Java.Util.Locale.Default.ISO3Language;
    }
}
