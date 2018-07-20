using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using FlashDevelop.Helpers;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace FlashDevelop.Managers
{
    static class ImageManager
    {
        const int Size16 = 16;
        const int Size32 = 32;
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
                Size = Size32;
                Padding = scale > 1.5 ? 2 : 1;
                Source = new Bitmap(FileNameHelper.Images32);
            }
            else
            {
                Size = Size16;
                Padding = 0;
                Source = new Bitmap(FileNameHelper.Images);
            }
        }

        /// <summary>
        /// Composes an icon from image data.
        /// </summary>
        public static Image GetComposedBitmap(string data, bool autoAdjusted)
        {
            var c = Components.Parse(data);
            string key = c.Key;

            if (!Cache.ContainsKey(key))
            {
                var original = new Bitmap(Size, Size);
                var srcRect = new Rectangle(0, 0, Size, Size);
                var destRect = new Rectangle(Padding, Padding, Size - (Padding * 2), Size - (Padding * 2));

                using (var graphics = Graphics.FromImage(original))
                {
                    graphics.Clear(Color.Transparent);

                    if (c.Icon >= 0)
                    {
                        srcRect.X = (c.Icon % Size16) * Size;
                        srcRect.Y = (c.Icon / Size16) * Size;
                        graphics.DrawImage(Source, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                    if (c.Bullet >= 0)
                    {
                        srcRect.X = (c.Bullet % Size16) * Size;
                        srcRect.Y = (c.Bullet / Size16) * Size;
                        destRect.X += (Size == Size32) ? c.X * 2 : c.X;
                        destRect.Y += (Size == Size32) ? c.Y * 2 : c.Y;
                        graphics.DrawImage(Source, destRect, srcRect, GraphicsUnit.Pixel);
                    }
                }

                original = ScaleHelper.Scale(original);
                Cache[key] = new ImagePair(original);
            }

            if (autoAdjusted)
            {
                var imagePair = Cache[key];
                return imagePair.Adjusted ?? AddAutoAdjustImage(imagePair);
            }
            return Cache[key].Original;
        }

        /// <summary>
        /// Composes an icon from image data with the size of 16x16
        /// </summary>
        public static Image GetComposedBitmapSize16(string data, bool autoAdjusted)
        {
            if (Size == Size16) return GetComposedBitmap(data, autoAdjusted);

            string key = Components.Parse(data, Size16).Key;
            if (!Cache.ContainsKey(key))
            {
                var image32 = GetComposedBitmap(data, false);
                int size = ScaleHelper.Scale(Size16);
                var image16 = new Bitmap(size, size);

                using (var graphics = Graphics.FromImage(image16))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(image32, 0, 0, image16.Width, image16.Height);
                }

                Cache[key] = new ImagePair(image16);
            }

            if (autoAdjusted)
            {
                var imagePair = Cache[key];
                return imagePair.Adjusted ?? AddAutoAdjustImage(imagePair);
            }
            return Cache[key].Original;
        }

        /// <summary>
        /// Gets an adjusted copy of the specified image.
        /// </summary>
        public static Image SetImageAdjustment(Image original)
        {
            int saturation, brightness;
            if (GetImageAdjustments(out saturation, out brightness))
            {
                return ImageKonverter.ImageAdjust(original, saturation, brightness);
            }
            return new Bitmap(original);
        }

        /// <summary>
        /// Gets a copy of the image that changes color according to the theme.
        /// </summary>
        public static Image GetAutoAdjustedImage(Image image)
        {
            for (int i = 0, length = AutoAdjusted.Count; i < length; i++)
            {
                var imagePair = AutoAdjusted[i];
                if (imagePair.Original == image)
                {
                    return imagePair.Adjusted ?? (imagePair.Adjusted = SetImageAdjustment(imagePair.Original));
                }
                if (imagePair.Adjusted == null)
                {
                    AutoAdjusted.RemoveAt(i--);
                    length--;
                }
            }
            return AddAutoAdjustImage(new ImagePair(image));
        }

        /// <summary>
        /// Adjust colors of all cached images.
        /// </summary>
        public static void AdjustAllImages()
        {
            int saturation, brightness;
            GetImageAdjustments(out saturation, out brightness);

            for (int i = 0, length = AutoAdjusted.Count; i < length; i++)
            {
                var imagePair = AutoAdjusted[i];
                var adjusted = imagePair.Adjusted;
                if (adjusted == null)
                {
                    AutoAdjusted.RemoveAt(i--);
                    length--;
                }
                else ImageKonverter.ImageAdjust(imagePair.Original, adjusted, saturation, brightness);
            }
        }

        /// <summary>
        /// Adds a pair to the update list.
        /// </summary>
        static Image AddAutoAdjustImage(ImagePair pair)
        {
            if (!AutoAdjusted.Contains(pair)) AutoAdjusted.Add(pair);
            return pair.Adjusted = SetImageAdjustment(pair.Original);
        }

        /// <summary>
        /// Gets the appropriate color adjustment components.
        /// </summary>
        static bool GetImageAdjustments(out int saturation, out int brightness)
        {
            switch (Globals.MainForm.GetThemeValue("ImageManager.ImageSet"))
            {
                default:
                case "Default": saturation =   0; brightness =   0; return false;
                case "Bright":  saturation =  20; brightness =   0; return true;
                case "Dim":     saturation =  -5; brightness =  -2; return true;
                case "Dark":    saturation =  -5; brightness = -10; return true;
                case "Darker":  saturation = -20; brightness = -20; return true;
                case "Black":   saturation = -50; brightness = -25; return true;
            }
        }

        /// <summary>
        /// Contains information parsed from an image data string.
        /// </summary>
        struct Components
        {
            public int Size;
            public int Icon;
            public int Bullet;
            public int X;
            public int Y;

            /// <summary>
            /// Returns a string to use as a dictionary key.
            /// </summary>
            public string Key
            {
                get { return Size + "_" + Icon + "|" + Bullet + "|" + X + "|" + Y; }
            }

            /// <summary>
            /// Parses an image data string with default size.
            /// </summary>
            public static Components Parse(string value)
            {
                return Parse(value, ImageManager.Size);
            }

            /// <summary>
            /// Parses an image data string with the specified size.
            /// </summary>
            public static Components Parse(string value, int size)
            {
                Components c;
                c.Size = size;
                ProcessImageData(value, out c.Icon, out c.Bullet, out c.X, out c.Y);
                return c;
            }

            /// <summary>
            /// Processes data from "icon|bullet|x|y"
            /// </summary>
            static void ProcessImageData(string data, out int icon, out int bullet, out int x, out int y)
            {
                icon = bullet = -1;
                x = y = 0;
                if (string.IsNullOrEmpty(data)) return;

                string[] args = data.Split('|');
                if (args.Length == 0 || !int.TryParse(args[0], out icon)) return;
                if (args.Length == 1 || !int.TryParse(args[1], out bullet)) return;
                if (bullet < 0 || args.Length < 4) return;
                int.TryParse(args[2], out x);
                int.TryParse(args[3], out y);
            }
        }

        /// <summary>
        /// A pair of images used for tracking original and adjusted.
        /// </summary>
        class ImagePair
        {
            Image original;
            WeakReference adjusted;

            /// <summary>
            /// Creates an instance of <see cref="ImagePair"/>.
            /// </summary>
            /// <param name="original"><see cref="Original"/></param>
            public ImagePair(Image original) : this(original, null)
            {
            }

            /// <summary>
            /// Creates an instance of <see cref="ImagePair"/>.
            /// </summary>
            /// <param name="original"><see cref="Original"/></param>
            /// <param name="adjusted"><see cref="Adjusted"/></param>
            public ImagePair(Image original, Image adjusted)
            {
                this.original = original;
                this.adjusted = new WeakReference(adjusted);
            }

            /// <summary>
            /// The original image.
            /// </summary>
            public Image Original
            {
                get { return original; }
            }

            /// <summary>
            /// The copy of <see cref="Original"/> that changes color according to the theme.
            /// </summary>
            public Image Adjusted
            {
                get { return adjusted.Target as Image; }
                set { adjusted.Target = value; }
            }
        }
    }

}
