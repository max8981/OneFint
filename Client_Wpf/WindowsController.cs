using ClientLibrary;
using CoreAudio;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Client_Wpf
{
    internal class WindowsController
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, int iParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutin, bool fResume);
        [DllImport("kernel32")]
        private static extern WindowsAPIs.ExecutionStateEnum SetThreadExecutionState(WindowsAPIs.ExecutionStateEnum esFlags);
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        private static extern void ShowCursor(int status);
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow([In] IntPtr hmonitor);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        private static readonly IntPtr _handle = new(0xffff);

        private static readonly IntPtr _currentMonitor = DisplayController.GetCurrentMonitor();
        private static readonly DisplayController.PHYSICAL_MONITOR[] _MONITORs = DisplayController.GetPhysicalMonitors(_currentMonitor);
        private static readonly MMDevice _playbackDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

        public static void ScreenPowerOff() => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        public static void ScreenPowerOn() => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
        public static void SetVolume(int volume)
        {
            if (_playbackDevice.AudioEndpointVolume != null)
            {
                volume = volume > 100 ? 100 : volume;
                volume = volume < 0 ? 0 : volume;
                _playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
            }
        }
        public static int GetVolume()
        {
            if (_playbackDevice.AudioEndpointVolume != null)
            {
                return (int)(_playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            }
            return 0;
        }
        public static void SetScreenBrightness(double brightness)
        {
            foreach (var _MONITOR in _MONITORs)
            {
                try
                {
                    DisplayController.SetMonitorBrightness(_MONITOR, brightness);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //ClientLibrary.ClientController.WriteLog("SetScreenBrightness", ex.Message);
                }
            }
        }
        public static void ShowCursor() => ShowCursor(0);
        public static void HiddenCursor() => ShowCursor(1);
        public static void PreventForCurrentThread(bool keepDisplayOn = true)
        {
            SetThreadExecutionState(keepDisplayOn
                ? WindowsAPIs.ExecutionStateEnum.Continuous | WindowsAPIs.ExecutionStateEnum.SystemRequired | WindowsAPIs.ExecutionStateEnum.DisplayRequired
                : WindowsAPIs.ExecutionStateEnum.Continuous | WindowsAPIs.ExecutionStateEnum.SystemRequired);
        }
        public static void RestoreForCurrentThread()
        {
            SetThreadExecutionState(WindowsAPIs.ExecutionStateEnum.Continuous);
        }
        public static double GetDpi(Window window)
        {
            var dpiForWindow = GetDpiForWindow(new WindowInteropHelper(window).Handle);
            return (double)dpiForWindow / 96.0;
        }
        public static void PowerOffAtDatePowerOn(DateTime date)
        {
            long duetime = date.ToFileTime();
            using SafeWaitHandle handle = CreateWaitableTimer(IntPtr.Zero, true, "MyWaitabletimer");
            if (SetWaitableTimer(handle, ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
            {
                using EventWaitHandle wh = new(false, EventResetMode.AutoReset);
                wh.SafeWaitHandle = handle;
                Process.Start("shutdown.exe", "-h");
                wh.WaitOne();
            }
        }
        public static void SaveConfiguration<T>(string key,T value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = configFile.AppSettings.Settings;
            var json = JsonSerializer.Serialize(value);
            if (setting[key] == null)
                setting.Add(key, json);
            else
                setting[key].Value = json;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        public static T LoadConfiguration<T>(string key) where T : new()
        {
            T? result;
            var json = ConfigurationManager.AppSettings[key];
            if (json != null)
            {
                result = JsonSerializer.Deserialize<T>(json);
                if (result != null)
                    return result;
            }
            return new();
        }
    }
}
