using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace SoundMixer
{
	internal static class IconHelper
	{
		public const int GCL_HICONSM = -34;
		public const int GCL_HICON = -14;

		public const int ICON_SMALL = 0;
		public const int ICON_BIG = 1;
		public const int ICON_SMALL2 = 2;

		public const int WM_GETICON = 0x7F;

		public static IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex)
		{
			if (IntPtr.Size > 4)
				return GetClassLongPtr64(hWnd, nIndex);
			else
				return new IntPtr(GetClassLongPtr32(hWnd, nIndex));
		}

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		public static extern uint GetClassLongPtr32(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		public static extern IntPtr GetClassLongPtr64(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
		static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		public static Icon GetAppIcon(Process process)
		{
			IntPtr hwnd = process.MainWindowHandle;
			IntPtr iconHandle = SendMessage(hwnd, WM_GETICON, ICON_BIG, 0);
			if (iconHandle == IntPtr.Zero)
				iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL, 0);
			if (iconHandle == IntPtr.Zero)
				iconHandle = SendMessage(hwnd, WM_GETICON, ICON_SMALL2, 0);
			if (iconHandle == IntPtr.Zero)
				iconHandle = GetClassLongPtr(hwnd, GCL_HICON);
			if (iconHandle == IntPtr.Zero)
				iconHandle = GetClassLongPtr(hwnd, GCL_HICONSM);

			if (iconHandle == IntPtr.Zero)
				return Icon.ExtractAssociatedIcon(process.MainModule.FileName);

			Icon icn = Icon.FromHandle(iconHandle);

			return icn;
		}
	}
}
