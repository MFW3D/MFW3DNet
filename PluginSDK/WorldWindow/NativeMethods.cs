using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace WorldWind.Interop
{
	//win32方法
	public sealed class NativeMethods
	{
		private NativeMethods()
		{
		}

		//多线程消息
		[StructLayout(LayoutKind.Sequential)]
		public struct Message
		{
			public IntPtr hWnd;
			public uint msg;
			public IntPtr wParam;
			public IntPtr lParam;
			public uint time;
			public System.Drawing.Point p;
		}

		//多线程消息
		[System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
	}
}
