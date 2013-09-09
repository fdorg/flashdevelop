using System;
using System.Runtime.InteropServices;
using ScintillaNet.Enums;

namespace ScintillaNet
{
	public class WinAPI
	{
		[DllImport("kernel32.dll")]
		public extern static IntPtr LoadLibrary(string lpLibFileName);
		
		[DllImport ("user32.dll")]
		public static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int width, int height, IntPtr hWndParent, int hMenu, IntPtr hInstance, string lpParam);
		
		[DllImport("kernel32.dll", EntryPoint = "SendMessage")]
		public static extern int SendMessageStr(IntPtr hWnd, int message, int data, string s);
		
		[DllImport("user32.dll")]
		public  static extern IntPtr SetFocus(IntPtr hwnd);
		
	}
	
}
