using System;
using System.Runtime.InteropServices;

namespace MFW3D
{
	public sealed class PerformanceTimer
	{
		#region 数据实例
		public static long TicksPerSecond;
		#endregion

		#region 创建
 		private PerformanceTimer() 
		{
		}
		static PerformanceTimer()
		{
			// 读取始终频率
			long tickFrequency = 0;
			if (!QueryPerformanceFrequency(ref tickFrequency))
				throw new NotSupportedException("The machine doesn't appear to support high resolution timer.");
			TicksPerSecond = tickFrequency;

			System.Diagnostics.Debug.WriteLine("tickFrequency = " + tickFrequency);
		}
		#endregion

		#region 高性能时钟

		[System.Security.SuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

		[System.Security.SuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		public static extern bool QueryPerformanceCounter(ref long PerformanceCount);

		#endregion
	}
}
