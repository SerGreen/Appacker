using ChangeIcon;
using System.Drawing;
using System.Drawing.IconLib;

namespace Appacker
{
    public static class IconSwapper
    {
        public static void ChangeIcon(string pathToTargetExe, string pathToExeWithIcon)
        {
            // Extract icon and save it into temp file

            Icon ico = Icon.ExtractAssociatedIcon(pathToExeWithIcon);
            MultiIcon mIco = new MultiIcon();
            SingleIcon sIco = mIco.Add("main");
            sIco.CreateFrom(ico.ToBitmap(), IconOutputFormat.Vista);
            string tempIcoPath = System.IO.Path.GetTempFileName();
            sIco.Save(tempIcoPath);

            // Magically inject icon into target exe
            IconInjector.InjectIcon(pathToTargetExe, tempIcoPath);

            // Delete temp ico file
            System.IO.File.Delete(tempIcoPath);
        }
    }
}
