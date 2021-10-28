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
    internal class IniSettingsProvider
    {
        internal bool isIniFilePresent;
        private IniData data;
        private static FileIniDataParser parser = new FileIniDataParser();

        public IniSettingsProvider ()
        {
            if (File.Exists("Appacker.ini"))
            {
                data = parser.ReadFile("Appacker.ini");
                isIniFilePresent = true;
            }
        }

        public Settings ReadIniSettings ()
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

                int.TryParse(data["Window"]["PositionTop"], out settings.positionTop);
                int.TryParse(data["Window"]["PositionLeft"], out settings.positionLeft);
                int.TryParse(data["Window"]["Width"], out settings.width);
                int.TryParse(data["Window"]["Height"], out settings.height);

                return settings;
            }

            return null;
        }

        public void SaveIniFile (Settings settings)
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

            data.Sections.AddSection("Window");
            data["Window"].AddKey("PositionTop", settings.positionTop.ToString());
            data["Window"].AddKey("PositionLeft", settings.positionLeft.ToString());
            data["Window"].AddKey("Width", settings.width.ToString());
            data["Window"].AddKey("Height", settings.height.ToString());

            parser.WriteFile("Appacker.ini", data);
            isIniFilePresent = true;
        }

    }
}
