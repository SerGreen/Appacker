using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacker
{
    internal static class RegistrySettingsProvider
    {
        public static CultureInfo Language
        {
            get => GetLanguage();
            set => SaveLanguage(value);
        }

        private static void SaveLanguage(CultureInfo lang)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\SerGreen\Appacker", true))
            {
                key.SetValue("Language", lang.ToString(), RegistryValueKind.String);
            }
        }

        private static CultureInfo GetLanguage()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\SerGreen\Appacker"))
            {
                string lang = key?.GetValue("Language") as string;
                if (lang == null)
                {
                    lang = "en-US";
                    SaveLanguage(CultureInfo.GetCultureInfo(lang));
                }

                return CultureInfo.GetCultureInfo(lang);
            }
        }
    }
}