using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static 屏幕管理.Systems.Screen;

namespace 屏幕管理.Systems
{
    internal class Screen
    {
        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const int PHYSICAL_MONITOR_DESCRIPTION_SIZE = 128;
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, int iParam);
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        private static extern IntPtr Keybd_Event(Keys keys, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("user32.dll", SetLastError = true)]
        private extern static bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll", SetLastError = false)]
        private extern static IntPtr MonitorFromPoint(POINT pt, uint dwFlags);
        [DllImport("user32.dll", SetLastError = false)]
        private extern static long SetDisplayConfig(uint numPathArrarElements, IntPtr pathArray, uint numModeArrayElements, IntPtr modeArray, uint flags);
        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, out uint pdwNumberOfPhysicalMonitors);
        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetPhysicalMonitorsFromHMONITOR(IntPtr hMonitor, uint dwPhysicalMonitorArraySize, [Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);
        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool GetMonitorBrightness(IntPtr hMonitor, out uint pdwMinimumBrightness, out uint pdwCurrentBrightness, out uint pdwMaximumBrightness);

        [DllImport("dxva2.dll", SetLastError = true)]
        private extern static bool SetMonitorBrightness(IntPtr hMonitor, uint dwNewBrightness);
        private static readonly IntPtr _currentMonitor = GetCurrentMonitor();
        private static readonly PHYSICAL_MONITOR[] _MONITORs = GetPhysicalMonitors(_currentMonitor);
        private static readonly IntPtr _handle = new(0xffff);
        private static bool _screenActivation = true;
        private static IntPtr GetCurrentMonitor()
        {
            if (!GetCursorPos(out POINT point))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
        }
        private static PHYSICAL_MONITOR[] GetPhysicalMonitors(IntPtr hMonitor)
        {
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(hMonitor, out uint dwNumberOfPhysicalMonitors))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            PHYSICAL_MONITOR[] physicalMonitorArray = new PHYSICAL_MONITOR[dwNumberOfPhysicalMonitors];
            if (!GetPhysicalMonitorsFromHMONITOR(hMonitor, dwNumberOfPhysicalMonitors, physicalMonitorArray))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return physicalMonitorArray;
        }
        private static double GetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor)
        {
            double result = 0;
            if (GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out uint dwMinimumBrightness, out uint dwCurrentBrightness, out uint dwMaximumBrightness))
                result = (dwCurrentBrightness - dwMinimumBrightness) / (double)(dwMaximumBrightness - dwMinimumBrightness);
            else
                Log.Default.Error(new Win32Exception(Marshal.GetLastWin32Error()), "GetMonitorBrightness", $"Parameter:{physicalMonitor}");
            return result;

        }
        private static void SetMonitorBrightness(PHYSICAL_MONITOR physicalMonitor, double brightness)
        {
            if (!GetMonitorBrightness(physicalMonitor.hPhysicalMonitor, out uint dwMinimumBrightness, out uint dwCurrentBrightness, out uint dwMaximumBrightness))
                Log.Default.Error(new Win32Exception(Marshal.GetLastWin32Error()), "SetMonitorBrightness", $"Parameter:{physicalMonitor}", $"Parameter_1:{brightness}");
            if (!SetMonitorBrightness(physicalMonitor.hPhysicalMonitor, (uint)(dwMinimumBrightness + (dwMaximumBrightness - dwMinimumBrightness) * brightness)))
                Log.Default.Error(new Win32Exception(Marshal.GetLastWin32Error()), "SetMonitorBrightness", $"Parameter:{physicalMonitor}", $"Parameter_1:{brightness}");
        }
        internal static bool ScreenActivation
        {
            get => _screenActivation;
            set
            {
                if (value) PowerOn();
                else PowerOff();
            }
        }
        internal static void PowerOff() {
            SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
            _screenActivation = false;
        }
        internal static void PowerOn() {
            Keybd_Event(Keys.Space, 0, 0, 0);
            SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
            _screenActivation = true;
        }
        internal static void SetBrightness(double brightness)
        {
            foreach (var _MONITOR in _MONITORs)
            {
                try
                {
                    SetMonitorBrightness(_MONITOR, brightness);
                }
                catch (Exception ex)
                {
                    Log.Default.Error(ex, "SetBrightness", $"Parameter:{brightness}");
                }
            }
        }
        internal static bool SetScreenMode(ScreenModeEnum screenMode)
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
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            int x;
            int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PHYSICAL_MONITOR
        {
            public IntPtr hPhysicalMonitor;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = PHYSICAL_MONITOR_DESCRIPTION_SIZE)]
            public char[] szPhysicalMonitorDescription;
        }
    }
}
