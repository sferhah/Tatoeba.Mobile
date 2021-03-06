﻿using Xamarin.Forms;

[assembly: Dependency(typeof(Tatoeba.Mobile.UWP.PlatformSpecific.Localize))]
namespace Tatoeba.Mobile.UWP.PlatformSpecific
{
    public class Localize : Mobile.PlatformSpecific.ILocalize
    {
        public string ThreeLetterISOLanguageName => System.Globalization.CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName;
    }
}
