using System.Drawing;
using System.Drawing.Imaging;

namespace ScintillaNet
{
    public static class RGBA
    {
        /// <summary>
        /// Converts Bitmap images to RGBA data.
        /// </summary>
        public static unsafe byte[] ConvertToRGBA(Bitmap image)
        {
            var rect = new Rectangle(0, 0, image.Width, image.Height);

            // Get the bitmap into a 32bpp ARGB format (if it isn't already)
            if (image.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var clone = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(clone))
                    graphics.DrawImage(image, rect);

                image = clone;
            }

            // Convert ARGB to RGBA
            var bitmapData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var size = bitmapData.Stride * bitmapData.Height;
            var bytes = new byte[size];
            try
            {
                var source = (byte*)bitmapData.Scan0;
                fixed (byte* destination = bytes)
                {
                    for (int offset = 0; offset < size; offset += 4)
                    {
                        // 4 bytes per pixel
                        destination[offset] = source[offset + 2]; // R
                        destination[offset + 1] = source[offset + 1]; // G
                        destination[offset + 2] = source[offset]; // B
                        destination[offset + 3] = source[offset + 3]; // A
                    }
                }
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }

            return bytes;
        }
    }
}
