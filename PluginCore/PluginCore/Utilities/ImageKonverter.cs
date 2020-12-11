// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using PluginCore.Managers;

namespace PluginCore.Utilities
{
    public class ImageKonverter
    {
        /// <summary>
        /// Converts image to an icon
        /// </summary>
        public static Icon ImageToIcon(Image image)
        {
            try
            {
                var bmp = new Bitmap(image);
                var hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Adjusts the saturation and brightness of the image.
        /// </summary>
        public static Image ImageAdjust(Image image, int saturation, int brightness)
        {
            Image result = new Bitmap(image.Width, image.Height);
            ImageAdjust(image, result, saturation, brightness);
            return result;
        }

        /// <summary>
        /// Adjusts the saturation and brightness of the image.
        /// </summary>
        public static void ImageAdjust(Image source, Image dest, int saturation, int brightness)
        {
            try
            {
                const float rwgt = 0.3086f;
                const float gwgt = 0.6094f;
                const float bwgt = 0.0820f;
                var sat = 1f + (saturation / 100f);
                var bri = 1f + (brightness / 100f);
                var baseSat = 1.0f - sat;
                var adjBrightness = bri - 1f;
                using var graphics = Graphics.FromImage(dest);
                // clear the destination before drawing the image
                graphics.Clear(Color.Transparent);
                var matrix = new ColorMatrix
                {
                    // adjust saturation
                    [0, 0] = baseSat * rwgt + sat,
                    [0, 1] = baseSat * rwgt,
                    [0, 2] = baseSat * rwgt,
                    [1, 0] = baseSat * gwgt,
                    [1, 1] = baseSat * gwgt + sat,
                    [1, 2] = baseSat * gwgt,
                    [2, 0] = baseSat * bwgt,
                    [2, 1] = baseSat * bwgt,
                    [2, 2] = baseSat * bwgt + sat,
                    // adjust brightness
                    [4, 0] = adjBrightness,
                    [4, 1] = adjBrightness,
                    [4, 2] = adjBrightness
                };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphics.DrawImage(source, new Rectangle(0, 0, dest.Width, dest.Height), 0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Converts image to a grayscale image
        /// </summary>
        public static Image ImageToGrayscale(Image image)
        {
            try
            {
                var result = new Bitmap(image.Width, image.Height);
                using var graphics = Graphics.FromImage(result);
                var matrix = new ColorMatrix(new[]
                {
                    new[] { 0.30f, 0.30f, 0.30f, 0, 0 },
                    new[] { 0.59f, 0.59f, 0.59f, 0, 0},
                    new[] { 0.11f, 0.11f, 0.11f, 0, 0},
                    new float[] { 0, 0, 0, 1, 0, 0},
                    new float[] { 0, 0, 0, 0, 1, 0},
                    new float[] { 0, 0, 0, 0, 0, 1 }
                });
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix);
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                return result;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return image;
            }
        }

        /// <summary>
        /// Resize image with GDI+ so that image is nice and clear with required size.
        /// </summary>
        public static Image ImageResize(Bitmap source, int width, int height)
        {
            var result = new Bitmap(width, height, source.PixelFormat);
            using var graphics = Graphics.FromImage(result);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            graphics.InterpolationMode = InterpolationMode.Bicubic;
            graphics.DrawImage(source, 0, 0, result.Width, result.Height);
            return result;
        }
    }
}