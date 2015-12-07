using System.Collections.Generic;
using System.Drawing;
using FlashDevelop.Helpers;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    static class ImageManager
    {
        static readonly int Size;
        static readonly int Padding;
        static readonly Bitmap Source;
        static readonly Dictionary<string, ImagePair> Cache;
        static readonly List<ImagePair> AutoAdjusted;

        /// <summary>
        /// Static constructor 
        /// </summary>
        static ImageManager()
        {
            double scale = ScaleHelper.GetScale();
            Cache = new Dictionary<string, ImagePair>();
            AutoAdjusted = new List<ImagePair>();

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
        }

        /// <summary>
        /// Composes an icon from image data.
        /// </summary>
        public static Image GetComposedBitmap(string data, bool autoAdjusted)
        {
            if (!Cache.ContainsKey(data))
            {
                int x, y, icon, bullet, rx, ry;
                var composed = new Bitmap(Size, Size);
                var destination = Graphics.FromImage(composed);
                var destRect = new Rectangle(Padding, Padding, Size - (Padding * 2), Size - (Padding * 2));

                ProcessImageData(data, out x, out y, out icon, out bullet);
                destination.Clear(Color.Transparent);

                if (icon >= 0)
                {
                    rx = (icon % 16) * Size;
                    ry = (icon / 16) * Size;
                    destination.DrawImage(Source, destRect, new Rectangle(rx, ry, Size, Size), GraphicsUnit.Pixel);
                }
                if (bullet >= 0)
                {
                    rx = (bullet % 16) * Size;
                    ry = (bullet / 16) * Size;
                    destRect.X += (Size == 32) ? x * 2 : x;
                    destRect.Y += (Size == 32) ? y * 2 : y;
                    destination.DrawImage(Source, destRect, new Rectangle(rx, ry, Size, Size), GraphicsUnit.Pixel);
                }

                composed = ScaleHelper.Scale(composed);
                Cache[data] = GetAutoAdjustedImagePair(composed);
            }

            return autoAdjusted ? Cache[data].Adjusted : Cache[data].Original;
        }

        /// <summary>
        /// Gets a copy of the image that changes color according to the theme.
        /// </summary>
        public static Image GetAutoAdjustedImage(Image image)
        {
            return GetAutoAdjustedImagePair(image).Adjusted;
        }

        /// <summary>
        /// Adjust colors of all cached images.
        /// </summary>
        public static void AdjustAllImages()
        {
            string style = Globals.MainForm.GetThemeValue("ImageManager.ImageSet");
            int saturation, brightness;
            GetImageAdjustments(style, out saturation, out brightness);
            
            foreach (var imagePair in AutoAdjusted)
            {
                ImageKonverter.ImageAdjust(imagePair.Original, imagePair.Adjusted, saturation, brightness);
            }
        }

        /// <summary>
        /// Processes data from "icon|bullet|x|y" or just index
        /// </summary>
        static void ProcessImageData(string data, out int x, out int y, out int icon, out int bullet)
        {
            x = y = 0;
            icon = bullet = -1;
            if (string.IsNullOrEmpty(data)) return;

            string[] args = data.Split('|');
            if (args.Length == 0 || !int.TryParse(args[0], out icon)) return;
            if (args.Length == 1 || !int.TryParse(args[1], out bullet)) return;
            if (bullet < 0 || args.Length < 4) return;
            int.TryParse(args[2], out x);
            int.TryParse(args[3], out y);
        }

        /// <summary>
        /// Gets an instance of <see cref="ImagePair"/> with the specified image,
        /// that has a copy of the image that changes color according to the theme.
        /// </summary>
        static ImagePair GetAutoAdjustedImagePair(Image image)
        {
            var pair = new ImagePair(image);
            AutoAdjusted.Add(pair);

            string style = Globals.MainForm.GetThemeValue("ImageManager.ImageSet");
            int saturation, brightness;

            if (GetImageAdjustments(style, out saturation, out brightness))
            {
                pair.Adjusted = new Bitmap(image.Width, image.Height);
                ImageKonverter.ImageAdjust(image, pair.Adjusted, saturation, brightness);
            }
            else pair.Adjusted = new Bitmap(image);

            return pair;
        }

        /// <summary>
        /// Gets the appropriate color adjustment components.
        /// </summary>
        static bool GetImageAdjustments(string style, out int saturation, out int brightness)
        {
            switch (style)
            {
                case "Bright": saturation =  20; brightness =   0; return true;
                case "Dim":    saturation =  -5; brightness =  -2; return true;
                case "Dark":   saturation =  -5; brightness = -10; return true;
                case "Darker": saturation = -20; brightness = -20; return true;
                case "Black":  saturation = -50; brightness = -25; return true;
                default:       saturation =   0; brightness =   0; return false;
            }
        }

        /// <summary>
        /// A pair of images used for tracking original and adjusted.
        /// </summary>
        class ImagePair
        {
            /// <summary>
            /// The original image.
            /// </summary>
            public Image Original;

            /// <summary>
            /// The copy of <see cref="Original"/> that changes color according to the theme.
            /// </summary>
            public Image Adjusted;

            /// <summary>
            /// Creates an instance of <see cref="ImagePair"/>.
            /// </summary>
            /// <param name="original"><see cref="Original"/></param>
            public ImagePair(Image original)
            {
                Original = original;
            }

            /// <summary>
            /// Creates an instance of <see cref="ImagePair"/>.
            /// </summary>
            /// <param name="original"><see cref="Original"/></param>
            /// <param name="adjusted"><see cref="Adjusted"/></param>
            public ImagePair(Image original, Image adjusted)
            {
                Original = original;
                Adjusted = adjusted;
            }
        }
    }

}
