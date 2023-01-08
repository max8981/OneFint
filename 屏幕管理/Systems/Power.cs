using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理.Systems
{
    internal class Power
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutin, bool fResume);
        internal static void Reboot() => Process.Start("shutdown.exe", "-r -f -t 0");
        internal static void ShutDown() => Process.Start("shutdown.exe", "-s -f -t 0");
        public static void HibernateTo(DateTime date)
        {
            var cmd = "powercfg.exe /hibernate on";
            Process.Start("cmd.exe", cmd);
            long duetime = date.ToFileTime();
            using SafeWaitHandle handle = CreateWaitableTimer(IntPtr.Zero, true, "MyWaitabletimer");
            if (SetWaitableTimer(handle, ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
            {
                using EventWaitHandle wh = new(false, EventResetMode.AutoReset);
                wh.SafeWaitHandle = handle;
                Process.Start("shutdown.exe", "-h");
                wh.WaitOne();
                Reboot();
            }
        }
    }
}
