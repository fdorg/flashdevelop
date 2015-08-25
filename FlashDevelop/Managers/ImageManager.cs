using System;
using System.Collections.Generic;
using System.Drawing;
using FlashDevelop.Helpers;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    class ImageManager
    {
        public static Int32 X;
        public static Int32 Y;
        public static Int32 Size;
        public static Int32 Icon;
        public static Int32 Bullet;
        public static Int32 Padding;
        public static Bitmap Source;
        public static Dictionary<String, Bitmap> Cache;

        /// <summary>
        /// Static constructor 
        /// </summary>
        static ImageManager()
        {
            Double scale = ScaleHelper.GetScale();
            Cache = new Dictionary<String, Bitmap>();
            if (scale >= 1.5)
            {
                Size = 32;
                Padding = scale > 1.5 ? 2 : 1;
                Source = new Bitmap(FileNameHelper.Images32);
            }
            else
            {
                Size = 16;
                Padding = 0;
                Source = new Bitmap(FileNameHelper.Images);
            }
            Source = (Bitmap)AdjustImage(Source);
        }

        /// <summary>
        /// Adjusts the image for different themes
        /// </summary>
        public static Image AdjustImage(Image image)
        {
            String style = Globals.MainForm.GetThemeValue("ImageManager.ImageSet");
            if (style == "Bright") return ImageKonverter.ImageAdjust(image, 20, 0);
            else if (style == "Dim") return ImageKonverter.ImageAdjust(image, -5, -2);
            else if (style == "Dark") return ImageKonverter.ImageAdjust(image, -5, -10);
            else if (style == "Darker") return ImageKonverter.ImageAdjust(image, -20, -20);
            else if (style == "Black") return ImageKonverter.ImageAdjust(image, -50, -25);
            else return image;
        }

        /// <summary>
        /// Composes an icon from Image data
        /// </summary>
        public static Bitmap GetComposedBitmap(String data)
        {
            if (Cache.ContainsKey(data))
            {
                return Cache[data];
            }
            ProcessImageData(data);
            Bitmap composed = new Bitmap(Size, Size);
            Graphics destination = Graphics.FromImage(composed);
            destination.Clear(Color.Transparent);
            Int32 rx; Int32 ry;
            if (Icon >= 0)
            {
                rx = (Icon % 16) * Size;
                ry = (Icon / 16) * Size;
                destination.DrawImage(Source, new Rectangle(Padding, Padding, Size - (Padding * 2), Size - (Padding * 2)), new Rectangle(rx, ry, Size, Size), GraphicsUnit.Pixel);
            }
            if (Bullet >= 0)
            {
                rx = (Bullet % 16) * Size;
                ry = (Bullet / 16) * Size;
                X = (Size == 32) ? X * 2 : X;
                Y = (Size == 32) ? Y * 2 : Y;
                destination.DrawImage(Source, new Rectangle(X + Padding, Y + Padding, Size - (Padding * 2), Size - (Padding * 2)), new Rectangle(rx, ry, Size, Size), GraphicsUnit.Pixel);
            }
            composed = ScaleHelper.Scale(composed);
            Cache[data] = composed;
            return composed;
        }

        /// <summary>
        /// Processes data from "icon|bullet|x|y" or just index
        /// </summary>
        private static void ProcessImageData(String data)
        {
            X = Y = 0;
            Icon = Bullet = -1;
            if (string.IsNullOrEmpty(data)) return;
            String[] par = data.Split('|');
            if (par.Length > 0)
            {
                Int32.TryParse(par[0], out Icon);
                if (par.Length > 1)
                {
                    Int32.TryParse(par[1], out Bullet);
                    if (Bullet >= 0 && par.Length == 4)
                    {
                        Int32.TryParse(par[2], out X);
                        Int32.TryParse(par[3], out Y);
                    }
                }
            }
        }

    }

}
