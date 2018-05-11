using ChangeIcon;
using System;
using System.Drawing;
using System.Drawing.IconLib;

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
            Bitmap bmp = GetIconFromFile(pathToFileWithIcon);
            if (bmp == null)
                return;

            ChangeIconFromBitmap(pathToTargetExe, bmp);
        }

        /// <summary>
        /// Returns a Bitmap of the icon associated with exe/dll or loads image file
        /// </summary>
        /// <param name="path">Full path to *.exe, *.dll or image file that will be used as the source for new icon</param>
        /// <returns></returns>
        public static Bitmap GetIconFromFile(string path)
        {
            Bitmap bmp = null;
            // Load the image based on file extension
            switch (System.IO.Path.GetExtension(path).ToLowerInvariant())
            {
                case ".exe":
                case ".dll":
                    bmp = Icon.ExtractAssociatedIcon(path).ToBitmap();
                    break;
                case ".jpg":
                case ".jpeg":
                case ".bmp":
                case ".png":
                case ".gif":
                case ".tiff":
                    bmp = (Bitmap)Image.FromFile(path);
                    break;
                case ".ico":
                    bmp = new Icon(path).ToBitmap();
                    break;
            }
            return bmp;
        }

        private static void ChangeIconFromBitmap(string pathToTargetExe, Bitmap bmpIcon)
        {
            if (bmpIcon.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb
                || bmpIcon.Width > 256
                || bmpIcon.Height > 256)
                bmpIcon = FixIcon(bmpIcon);

            // Create icon and save it into temp file
            MultiIcon mIco = new MultiIcon();
            SingleIcon sIco = mIco.Add("main");
            sIco.CreateFrom(bmpIcon, IconOutputFormat.Vista);
            string tempIcoPath = System.IO.Path.GetTempFileName();
            sIco.Save(tempIcoPath);

            // Magically inject icon into target exe
            IconInjector.InjectIcon(pathToTargetExe, tempIcoPath);

            // Delete temp ico file
            System.IO.File.Delete(tempIcoPath);
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
