using System;
using System.Runtime.InteropServices;

namespace MFW3D
{
	public sealed class PerformanceTimer
	{
		#region ����ʵ��
		public static long TicksPerSecond;
		#endregion

		#region ����
 		private PerformanceTimer() 
		{
		}
		static PerformanceTimer()
		{
			// ��ȡʼ��Ƶ��
			long tickFrequency = 0;
			if (!QueryPerformanceFrequency(ref tickFrequency))
				throw new NotSupportedException("The machine doesn't appear to support high resolution timer.");
			TicksPerSecond = tickFrequency;

			System.Diagnostics.Debug.WriteLine("tickFrequency = " + tickFrequency);
		}
		#endregion

		#region ������ʱ��

		[System.Security.SuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

		[System.Security.SuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		public static extern bool QueryPerformanceCounter(ref long PerformanceCount);

		#endregion
	}
}
