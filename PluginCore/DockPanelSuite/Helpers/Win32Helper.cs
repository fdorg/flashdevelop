// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class Win32Helper
    {
        public static Control ControlAtPoint(Point pt)
        {
            return Control.FromChildHandle(NativeMethods.WindowFromPoint(pt));
        }

        public static uint MakeLong(int low, int high)
        {
            return (uint)((high << 16) + low);
        }
    }
}
