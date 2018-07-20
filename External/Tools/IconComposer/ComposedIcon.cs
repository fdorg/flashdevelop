// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;

namespace IconComposer
{
    class ComposedIcon
    {
        public int icon;
        public int bullet;
        public int x;
        public int y;

        public ComposedIcon()
        {
            Clear();
        }

        public void Clear()
        {
            icon = bullet = -1;
            x = y = 0;
        }

        /// <summary>
        /// Compose an icon
        /// </summary>
        public Bitmap GetComposedBitmap(Image src)
        {
            Bitmap composed = new Bitmap(16, 16);
            Graphics dest = Graphics.FromImage(composed);
            dest.Clear(Color.Transparent);

            int rx;
            int ry;
            if (icon >= 0)
            {
                rx = (icon % 16) * 16;
                ry = (icon / 16) * 16;
                dest.DrawImage(src, new Rectangle(0, 0, 16, 16), new Rectangle(rx, ry, 16, 16), GraphicsUnit.Pixel);
            }
            if (bullet >= 0)
            {
                rx = (bullet % 16) * 16;
                ry = (bullet / 16) * 16;
                dest.DrawImage(src, new Rectangle(x, y, 16, 16), new Rectangle(rx, ry, 16, 16), GraphicsUnit.Pixel);
            }

            return composed;
        }

        /// <summary>
        /// Deserialize from "icon|bullet|x|y"
        /// </summary>
        /// <param name="str"></param>
        public void deserialize(string str)
        {
            Clear();
            if (str == null || str.Length == 0) return;

            string[] par = str.Split('|');
            if (par.Length > 0)
            {
                int.TryParse(par[0], out icon);
                if (icon >= 0 && par.Length > 1)
                {
                    int.TryParse(par[1], out bullet);
                    if (bullet >= 0 && par.Length == 4)
                    {
                        int.TryParse(par[2], out x);
                        int.TryParse(par[3], out y);
                    }
                }
            }
        }

        /// <summary>
        /// Serialize to "icon|bullet|x|y"
        /// </summary>
        public override string ToString()
        {
            if (bullet < 0) return (icon > 0) ? icon.ToString() : "";
            else return String.Format("{0}|{1}|{2}|{3}", icon, bullet, x, y);
        }
    }
}
