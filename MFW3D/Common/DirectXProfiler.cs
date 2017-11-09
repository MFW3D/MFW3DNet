using System;
using System.Diagnostics;
using Microsoft.DirectX;
using System.Runtime.InteropServices;

namespace MFW3D
{
    /// <summary>
    /// 一个提高性能的事件探测工具集合
    /// </summary>
    public class DirectXProfilerEvent : IDisposable
    {
        public DirectXProfilerEvent(System.Drawing.Color color, string name)
        {
            DirectXProfiler.BeginEvent(color, name);
        }
        public DirectXProfilerEvent(string name)
        {
            DirectXProfiler.BeginEvent(name);
        }
        public DirectXProfilerEvent()
        {
            DirectXProfiler.BeginEvent();
        }
        public void Dispose()
        {
            DirectXProfiler.EndEvent();
        }
    }

    /// <summary>
    /// 静态方法类
    /// </summary>
    public class DirectXProfiler
    {
        #region 事件探测

#if DEBUG
        private static bool enabled = true;
#else
            private static bool enabled = false;
#endif
        /// <summary>
        /// Whether or not the profiler output is enabled. You should set this
        /// to false for release, since there's no point in draaing your app down
        /// with profiler info calls.
        /// </summary>
        public static bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        private static bool addLineInfo = false;
        /// <summary>
        /// Whether or not to add code-file line info to the events &amp; markers sent
        /// to the profiler. This can be useful for debugging large applications,
        /// but as it is very costly and may be a big hit on performance you may
        /// want to disable it.
        /// </summary>
        public static bool AddLineInfo
        {
            get { return addLineInfo; }
            set { addLineInfo = value; }
        }

        #endregion

        #region Profiler event and marker methods

        public static int BeginEvent()
        {
            return BeginEvent(" ");
        }

        public static int BeginEvent(string name)
        {
            return BeginEvent(System.Drawing.Color.Black, name);
        }

        public static int BeginEvent(System.Drawing.Color color, string name)
        {
            return BeginEvent(unchecked((uint)color.ToArgb()), name);
        }

        public static void SetMarker()
        {
            SetMarker(" ");
        }

        public static void SetMarker(string name)
        {
            SetMarker(System.Drawing.Color.Black, name);
        }

        public static void SetMarker(System.Drawing.Color color, string name)
        {
            SetMarker(unchecked((uint)color.ToArgb()), name);
        }

        public static int BeginEvent(uint col, string name)
        {
            if (enabled)
            {
                return BeginEventDirect(col, name + GetLineInfo());
            }
            else
            {
                return -1;
            }
        }

        public static int EndEvent()
        {
            if (enabled)
            {
                return EndEventDirect();
            }
            else
            {
                return -1;
            }
        }

        public static void SetMarker(uint col, string name)
        {
            if (enabled)
            {
                SetMarkerDirect(col, name + GetLineInfo());
                GetLineInfo();
            }
        }

        #endregion

        #region Line tracing and DLL imports

        private static string GetLineInfo()
        {
            if (addLineInfo)
            {
                StackTrace trace = new StackTrace(true);

                int i = 0;
                StackFrame frame = trace.GetFrame(i);
                string lastFile = System.IO.Path.GetFileName(frame.GetFileName()).ToLower();

                // ugly hack to trace out off profiler class
                while (lastFile == "profiler.cs" && i < trace.FrameCount)
                {
                    i++;
                    frame = trace.GetFrame(i);
                    lastFile = System.IO.Path.GetFileName(frame.GetFileName()).ToLower();
                }

                return String.Format(" ({0}:{1})", lastFile, frame.GetFileLineNumber());
            }
            else
            {
                return string.Empty;
            }
        }

        // int D3DPERF_BeginEvent( D3DCOLOR col, LPCWSTR wszName );		
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("d3d9.dll", EntryPoint = "D3DPERF_BeginEvent", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        private static extern int BeginEventDirect(uint col, string wszName);

        // int D3DPERF_EndEvent();
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously, really 
        [DllImport("d3d9.dll", EntryPoint = "D3DPERF_EndEvent", CallingConvention = CallingConvention.Winapi)]
        private static extern int EndEventDirect();

        // void D3DPERF_SetMarker( D3DCOLOR col,LPCWSTR wszName);
        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously, really really
        [DllImport("d3d9.dll", EntryPoint = "D3DPERF_SetMarker", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        private static extern void SetMarkerDirect(uint col, string wszName);

        #endregion
    }
}
