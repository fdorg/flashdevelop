using System;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
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
                Bitmap bmp = new Bitmap(image);
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Converts image to a grayscale image
        /// </summary>
        public static Image ImageToGrayscale(Image image)
        {
            try
            {
                Bitmap bitmap = new Bitmap(image.Width, image.Height);
                Graphics graphics = Graphics.FromImage(bitmap);
                ColorMatrix matrix = new ColorMatrix(new float[][]{   
                    new float[]{0.3f,0.3f,0.3f,0,0},
                    new float[]{0.59f,0.59f,0.59f,0,0},
                    new float[]{0.11f,0.11f,0.11f,0,0},
                    new float[]{0,0,0,1,0,0},
                    new float[]{0,0,0,0,1,0},
                    new float[]{0,0,0,0,0,1}});
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix);
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                graphics.Dispose(); // Dispose temp graphics
                return (Image)bitmap;
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
        public static Image ImageResize(Bitmap source, Int32 width, Int32 height)
        {
            Bitmap bitmap = new Bitmap(width, height, source.PixelFormat);
            Graphics graphicsImage = Graphics.FromImage(bitmap);
            graphicsImage.SmoothingMode = SmoothingMode.HighQuality;
            graphicsImage.PixelOffsetMode = PixelOffsetMode.Half;
            graphicsImage.InterpolationMode = InterpolationMode.Bicubic;
            graphicsImage.DrawImage(source, 0, 0, bitmap.Width, bitmap.Height);
            graphicsImage.Dispose();
            return bitmap;
        }

    }

}

