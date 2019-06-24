using ChangeIcon;
using System;
using System.Drawing;
using System.Drawing.IconLib;
using System.IO;

namespace Appacker
{
    public static class IconSwapper
    {
        /// <summary>
        /// Replaces icon of the target exe with the new one
        /// </summary>
        /// <param name="pathToTargetExe">Full path to the executable, the icon of which will be replaced</param>
        /// <param name="pathToFileWithIcon">Full path to *.exe, *.dll or image file that will be used as the source for new icon</param>
        public static void ChangeIcon(string pathToTargetExe, string pathToFileWithIcon)
        {
            // Load the image based on file extension
            SingleIcon ico = GetIconFromFile(pathToFileWithIcon);

            // If icon is empty
            if (ico.Count == 0)
                return;

            InjectIcon(pathToTargetExe, ico);
        }

        /// <summary>
        /// Returns a Bitmap of the icon associated with exe/dll or loads image file
        /// </summary>
        /// <param name="path">Full path to *.exe, *.dll or image file that will be used as the source for new icon</param>
        /// <returns></returns>
        public static SingleIcon GetIconFromFile(string path)
        {
            MultiIcon mIco = new MultiIcon();
            SingleIcon ico = mIco.Add("Icon1");
            // Load the image based on file extension
            switch (Path.GetExtension(path).ToLowerInvariant())
            {
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".png":
                case ".gif":
                case ".tiff":
                    ico = ImageToIcon(path);
                    break;
                case ".exe":
                case ".dll":
                case ".ico":
                    mIco.Load(path);
                    // If icon pack has multiple icons, take the first one
                    if (mIco.Count > 0)
                        ico = mIco[0];
                    // If exe doesn't have any icon it will not load the 'default' icon
                    // But .NET Icon class can actually extract this 'default exe' icon
                    else if (ico.Count == 0)
                        ico.CreateFrom(Icon.ExtractAssociatedIcon(path).ToBitmap(), IconOutputFormat.Vista);
                        // Tip: you have to convert to Bitmap in order to get 16mil colors
                        // if you load directly from icon it gets only 16 colors for some reason

                    break;
            }
            return ico;
        }

        private static SingleIcon ImageToIcon(string imgPath)
        {
            Bitmap bmp = (Bitmap)Image.FromFile(imgPath);
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb
                || bmp.Width > 256
                || bmp.Height > 256)
                bmp = FixIcon(bmp);

            MultiIcon mIco = new MultiIcon();
            SingleIcon sIco = mIco.Add("Icon1");
            sIco.CreateFrom(bmp, IconOutputFormat.Vista);
            return sIco;
        }
                
        private static void InjectIcon(string pathToTargetExe, SingleIcon icon)
        {
            // Save icon to a temp file
            string tempIcoPath = Path.GetTempFileName();
            icon.Save(tempIcoPath);

            // Magically inject icon into the target exe
            IconInjector.InjectIcon(pathToTargetExe, tempIcoPath);

            // Delete temp ico file
            File.Delete(tempIcoPath);
        }

        private static Bitmap FixIcon(Bitmap orig)
        {
            float scale = Math.Min(256f / orig.Width, 256f / orig.Height);
            int cloneWidth = (int)(orig.Width * scale);
            int cloneHeight = (int)(orig.Height * scale);
            
            Bitmap clone = new Bitmap(cloneWidth, cloneHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(clone))
                g.DrawImage(orig, new Rectangle(0, 0, clone.Width, clone.Height));

            orig.Dispose();
            return clone;
        }
    }
}
