﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Client_Wpf
{
    internal class DisplayController
    {
        private const int MONITOR_DEFAULTTONEAREST = 2;

        private const int PHYSICAL_MONITOR_DESCRIPTION_SIZE = 128;

        private const int MC_CAPS_BRIGHTNESS = 0x2;

        [StructLayout(LayoutKind.Sequential)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = PHYSICAL_MONITOR_DESCRIPTION_SIZE)]
            public char[] szPhysicalMonitorDescription;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private extern static bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = false)]
        private extern static IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("user32.dll", SetLastError = false)]
        private extern static long SetDisplayConfig(uint numPathArrarElements,IntPtr pathArray,uint numModeArrayElements,IntPtr modeArray,uint flags);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool DestroyPhysicalMonitors(uint dwPhysicalMonitorArraySize, PHYSICAL_MONITOR[] pPhysicalMonitorArray);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorCapabilities(IntPtr hMonitor, out uint pdwMonitorCapabilities, out uint pdwSupportedColorTemperatures);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);

        public static IntPtr GetCurrentMonitor()
        {
            POINT point = new POINT();
            if (!GetCursorPos(out point))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
        }

        public static PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr hMonitor)
        {
            uint dwNumberOfPhysicalMonitors;
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out dwNumberOfPhysicalMonitors))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            PHYSICAL_MONITOR[] physicalMonitorArray = new PHYSICAL_MONITOR[dwNumberOfPhysicalMonitors];
            if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, dwNumberOfPhysicalMonitors, physicalMonitorArray))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return physicalMonitorArray;
        }

        public static void DestroyPhysicalMonitors(PHYSICAL_MONITOR[] physicalMonitorArray)
        {
            if (!DestroyPhysicalMonitors((uint)physicalMonitorArray.Length, physicalMonitorArray))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static uint GetMonitorCapabilities(PHYSICAL_MONITOR physicalMonitor)
        {
            uint dwMonitorCapabilities, dwSupportedColorTemperatures;
            if (!GetMonitorCapabilities(physicalMonitor.hPhysicalMonitor, out dwMonitorCapabilities, out dwSupportedColorTemperatures))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return dwMonitorCapabilities;
        }

        public static bool GetBrightnessSupport(PHYSICAL_MONITOR physicalMonitor)
        {
            return (GetMonitorCapabilities(physicalMonitor) & MC_CAPS_BRIGHTNESS) != 0;
        }

        public static double GetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor)
        {
            if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out uint dwMinimumBrightness, out uint dwCurrentBrightness, out uint dwMaximumBrightness))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            return (double)(dwCurrentBrightness - dwMinimumBrightness) / (double)(dwMaximumBrightness - dwMinimumBrightness);
        }

        public static void SetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor, double brightness)
        {
            uint dwMinimumBrightness, dwCurrentBrightness, dwMaximumBrightness;
            if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out dwMinimumBrightness, out dwCurrentBrightness, out dwMaximumBrightness))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            if (!SetMonitorBrightness(physicalMonitor.hPhysicalMonitor, (uint)(dwMinimumBrightness + (dwMaximumBrightness - dwMinimumBrightness) * brightness)))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public static bool SetScreenMode(ScreenModeEnum screenMode)
        {
            var mode = screenMode switch
            {
                ScreenModeEnum.主屏 => (uint)(0x00000080 | 0x00000001),
                ScreenModeEnum.复制 => (uint)(0x00000080 | 0x00000002),
                ScreenModeEnum.扩展 => (uint)(0x00000080 | 0x00000004),
                _ => (uint)(0x00000080 | 0x00000001),
            };
            return SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, mode) == 0;
        }
        public enum ScreenModeEnum
        {
            主屏,
            复制,
            扩展,
        }
    }
}
   