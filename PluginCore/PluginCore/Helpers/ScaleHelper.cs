using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Utilities;

namespace PluginCore.Helpers
{
    public class ScaleHelper
    {
        /// <summary>
        /// Private properties
        /// </summary>
        private static double curScale = double.MinValue;
        private static HashSet<Control> adjustedItems = new HashSet<Control>();

        /// <summary>
        /// Gets the display scale. Ideally would probably keep separate scales for X and Y.
        /// </summary>
        public static double GetScale()
        {
            if (curScale != double.MinValue) return curScale;
            using (var g = Graphics.FromHwnd(PluginBase.MainForm.Handle))
            {
                curScale = g.DpiX / 96f;
            }
            return curScale;
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
            if (GetScale() == 1) return image;
            int width = Scale(image.Width);
            int height = Scale(image.Height);
            return (Bitmap)ImageKonverter.ImageResize(image, width, height);
        }

        /// <summary>
        /// Adjusts the specified control for better high dpi look
        /// </summary>
        public static void AdjustForHighDPI(Control control, double multi)
        {
            if (IsAdjusted(control)) return;
            double scale = GetScale();
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is Button)
                {
                    if (scale >= 1.5)
                    {
                        double noPad = ctrl.Height * multi;
                        ctrl.Height = (Int32)noPad;
                    }
                }
                AdjustForHighDPI(ctrl, multi);
            }
        }
        public static void AdjustForHighDPI(Control control)
        {
            AdjustForHighDPI(control, 0.92);
        }

        /// <summary>
        /// Keep track and adjust forms only once
        /// </summary>
        private static Boolean IsAdjusted(Control control)
        {
            if (control is Form)
            {
                if (adjustedItems.Contains(control)) return true;
                else
                {
                    adjustedItems.Add(control);
                    return false;
                }
            }
            else return false;
        }

    }

}
