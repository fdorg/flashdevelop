using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace PluginCore.Helpers
{
    public class ScaleHelper
    {
        private static double _scale = double.MinValue;

        /// <summary>
        /// Gets the display scale. Ideally would probably keep separate scales for X and Y.
        /// </summary>
        private static double GetScale()
        {
            if (_scale != double.MinValue)
                return _scale;

            using (var g = Graphics.FromHwnd(PluginBase.MainForm.Handle))
            {
                _scale = g.DpiX / 96f;
            }

            return _scale;
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static int Scale(int value)
        {
            return (int)(value * GetScale());
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static long Scale(long value)
        {
            return (long)(value * GetScale());
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static float Scale(float value)
        {
            return (float)(value * GetScale());
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static double Scale(double value)
        {
            return value * GetScale();
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static Size Scale(Size value)
        {
            return new Size(Scale(value.Width), Scale(value.Height));
        }

        /// <summary>
        /// Resizes based on display scale.
        /// </summary>
        public static SizeF Scale(SizeF value)
        {
            return new SizeF(Scale(value.Width), Scale(value.Height));
        }

        /// <summary>
        /// Resizes the image based on the display scale. Uses high quality settings.
        /// </summary>
        public static Bitmap Scale(Bitmap image)
        {
            if (GetScale() == 1)
                return image;

            if (GetScale() >= 2)
                return Stretch(image);

            int width = Scale(image.Width);
            int height = Scale(image.Height);

            return (Bitmap)ImageKonverter.ImageResize(image, width, height);
        }

        /// <summary>
        /// Resizes the image based on the display scale. Uses default quality settings. Necessary to not break the DockPanel bitmaps.
        /// </summary>
        public static Bitmap Stretch(Bitmap image)
        {
            if (GetScale() == 1)
                return image;

            int width = Scale(image.Width);
            int height = Scale(image.Height);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphicsImage = Graphics.FromImage(bitmap);
            graphicsImage.SmoothingMode = SmoothingMode.HighQuality;
            graphicsImage.InterpolationMode = InterpolationMode.NearestNeighbor;

            // use an image attribute in order to remove the black/gray border around image after resize
            // (most obvious on white images), see this post for more information:
            // http://www.codeproject.com/KB/GDI-plus/imgresizoutperfgdiplus.aspx
            using (var attribute = new ImageAttributes())
            {
                attribute.SetWrapMode(WrapMode.TileFlipXY);

                // draws the resized image to the bitmap
                graphicsImage.DrawImage(image, new Rectangle(new Point(0, 0), bitmap.Size), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attribute);
            }

            graphicsImage.Dispose();
            return bitmap;
        }
    }
}
