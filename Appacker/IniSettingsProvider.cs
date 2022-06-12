using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace Appacker
{
    internal static class IniSettingsProvider
    {
        [Flags]
        internal enum IniLocationFlags { Local = 1, AppData = 2 };
        readonly private static string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Appacker");
        readonly static string appDataPath = Path.Combine(appDataFolder, "Appacker.ini");
        readonly static string localIniFileName = "Appacker.ini";

        private static IniData data;
        private static FileIniDataParser parser = new FileIniDataParser();
        private static IniLocationFlags iniLocation;

        internal static bool isLocalIniFilePresent => iniLocation.HasFlag(IniLocationFlags.Local);
        internal static bool isAppDataIniFilePresent => iniLocation.HasFlag(IniLocationFlags.AppData);

        static IniSettingsProvider ()
        {
            if (File.Exists(appDataPath))
                iniLocation |= IniLocationFlags.AppData;

            if (File.Exists(localIniFileName))
                iniLocation |= IniLocationFlags.Local;

            if (iniLocation.HasFlag(IniLocationFlags.Local))
                data = parser.ReadFile(localIniFileName);
            else if (iniLocation.HasFlag(IniLocationFlags.AppData))
                data = parser.ReadFile(appDataPath);
        }

        public static Settings ReadIniSettings ()
        {
            if (data != null)
            {
                Settings settings = new Settings();

                settings.language = data["General"]["Language"];

                bool.TryParse(data["AdvancedOptions"]["SelfRepack"], out settings.isRepackable);
                Enum.TryParse<MainForm.UnpackDirectory>(data["AdvancedOptions"]["UnpackDirectory"], out settings.unpackDirectory);
                bool.TryParse(data["AdvancedOptions"]["OpenUnpackDirectory"], out settings.openUnpackDirectory);
                settings.launchArguments = data["AdvancedOptions"]["CmdArguments"];
                settings.customFileDescription = data["AdvancedOptions"]["FileDescription"];
                bool.TryParse(data["AdvancedOptions"]["WindowlessUnpacker"], out settings.isWindowless);
                bool.TryParse(data["AdvancedOptions"]["SplashScreenProgressBar"], out settings.isSplashProgressBarEnabled);

                int.TryParse(data["Window"]["PositionTop"], out settings.positionTop);
                int.TryParse(data["Window"]["PositionLeft"], out settings.positionLeft);
                int.TryParse(data["Window"]["Width"], out settings.width);
                int.TryParse(data["Window"]["Height"], out settings.height);

                return settings;
            }

            return null;
        }

        public static void SaveIniFile (Settings settings, IniLocationFlags location)
        {
            IniData data = new IniData();

            data.Sections.AddSection("General");
            data["General"].AddKey("Language", settings.language);

            data.Sections.AddSection("AdvancedOptions");
            data["AdvancedOptions"].AddKey("SelfRepack", settings.isRepackable.ToString());
            data["AdvancedOptions"].AddKey("UnpackDirectory", settings.unpackDirectory.ToString());
            data["AdvancedOptions"].AddKey("OpenUnpackDirectory", settings.openUnpackDirectory.ToString());
            data["AdvancedOptions"].AddKey("CmdArguments", settings.launchArguments);
            data["AdvancedOptions"].AddKey("FileDescription", settings.customFileDescription);
            data["AdvancedOptions"].AddKey("WindowlessUnpacker", settings.isWindowless.ToString());
            data["AdvancedOptions"].AddKey("SplashScreenProgressBar", settings.isSplashProgressBarEnabled.ToString());

            data.Sections.AddSection("Window");
            data["Window"].AddKey("PositionTop", settings.positionTop.ToString());
            data["Window"].AddKey("PositionLeft", settings.positionLeft.ToString());
            data["Window"].AddKey("Width", settings.width.ToString());
            data["Window"].AddKey("Height", settings.height.ToString());

            if (location.HasFlag(IniLocationFlags.Local))
            {
                parser.WriteFile(localIniFileName, data);
                iniLocation |= IniLocationFlags.Local;
            }
            else if (location.HasFlag(IniLocationFlags.AppData))
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                parser.WriteFile(appDataPath, data);
                iniLocation |= IniLocationFlags.AppData;
            }
        }

        public static void DeleteIniFile (IniLocationFlags location)
        {
            if (location.HasFlag(IniLocationFlags.Local))
            {
                if (File.Exists(localIniFileName))
                    File.Delete(localIniFileName);
                iniLocation &= ~IniLocationFlags.Local;
            }

            if (location.HasFlag(IniLocationFlags.AppData))
            {
                if (File.Exists(appDataPath))
                    File.Delete(appDataPath);
                iniLocation &= ~IniLocationFlags.AppData;
            }
        }

    }
}
