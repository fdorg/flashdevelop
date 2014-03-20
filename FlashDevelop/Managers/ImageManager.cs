using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FlashDevelop.Helpers;
using PluginCore.Helpers;

namespace FlashDevelop.Managers
{
    class ImageManager
    {
        public static Int32 X;
        public static Int32 Y;
        public static Int32 Icon;
        public static Int32 Bullet;
        public static Bitmap Source;
        public static Dictionary<String, Bitmap> Cache;

        static ImageManager()
        {
            Cache = new Dictionary<String, Bitmap>();
            Source = new Bitmap(FileNameHelper.Images);
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
            Bitmap composed = new Bitmap(16, 16);
            Graphics destination = Graphics.FromImage(composed);
            destination.Clear(Color.Transparent);
            Int32 rx; Int32 ry;
            if (Icon >= 0)
            {
                rx = (Icon % 16) * 16; 
                ry = (Icon / 16) * 16;
                destination.DrawImage(Source, new Rectangle(0, 0, 16, 16), new Rectangle(rx, ry, 16, 16), GraphicsUnit.Pixel);
            }
            if (Bullet >= 0)
            {
                rx = (Bullet % 16) * 16; 
                ry = (Bullet / 16) * 16;
                destination.DrawImage(Source, new Rectangle(X, Y, 16, 16), new Rectangle(rx, ry, 16, 16), GraphicsUnit.Pixel);
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
            if (data == null || data.Length == 0) return;
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
