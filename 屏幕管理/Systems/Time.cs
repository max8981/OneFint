using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using 屏幕管理.ServerToClient;

namespace 屏幕管理.Systems
{
    internal class Time
    {
        [DllImport("kernel32")]
        private static extern bool SetSystemTime(ref SystemTime systemTime);
        [DllImport("kernel32")]
        private static extern bool SetLocalTime(ref SystemTime systemTime);
        private static long _forwardSecond;
        internal static DateTime Now => DateTime.Now.AddSeconds(_forwardSecond);
        internal static void SetDate(long forwardSecond)
        {
            var date = DateTime.Now.AddSeconds(forwardSecond);
            if (!SetDate(date))
                _forwardSecond = forwardSecond;
        }
        internal static bool SetDate(DateTime date)
        {
            SystemTime time = new()
            {
                year = (ushort)date.Year,
                month = (ushort)date.Month,
                dayOfWeek = (ushort)date.DayOfWeek,
                day = (ushort)date.Day,
                hour = (ushort)date.Hour,
                minute = (ushort)date.Minute,
                second = (ushort)date.Second,
                milliseconds = (ushort)date.Millisecond,
            };

            return SetLocalTime(ref time);
        }
        [StructLayout(LayoutKind.Sequential)]
        struct SystemTime
        {
            [MarshalAs(UnmanagedType.U2)]
            internal ushort year;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort month;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort dayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort day;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort hour;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort minute;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort second;
            [MarshalAs(UnmanagedType.U2)]
            internal ushort milliseconds;
        }
    }
}
