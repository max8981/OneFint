using AutoUpdaterDotNET;
using ClientLibrary;
using CoreAudio;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using PP.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Client_Wpf
{
    internal class WindowsController
    {
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, int iParam);
        [DllImport("user32.dll",EntryPoint ="keybd_event",SetLastError =true)]
        private static extern IntPtr Keybd_Event(Keys keys, byte bScan, uint dwFlags, uint dwExtraInfo);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutin, bool fResume);
        [DllImport("kernel32")]
        private static extern WindowsAPIs.ExecutionStateEnum SetThreadExecutionState(WindowsAPIs.ExecutionStateEnum esFlags);
        [DllImport("kernel32")]
        private static extern bool SetSystemTime(ref SystemTime systemTime);
        [DllImport("kernel32")]
        private static extern bool SetLocalTime(ref SystemTime systemTime);
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        private static extern void ShowCursor(int status);
        [DllImport("user32.dll")]
        private static extern uint GetDpiForWindow([In] IntPtr hmonitor);
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError =true)]
        private static extern bool ShowWindow(IntPtr hWnd,uint nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint WM_DEVICECHANGE = 0x0219;
        private const uint SC_MONITORPOWER = 0xF170;
        private static readonly IntPtr _handle = new(0xffff);

        private static readonly IntPtr _currentMonitor = DisplayController.GetCurrentMonitor();
        private static readonly DisplayController.PHYSICAL_MONITOR[] _MONITORs = DisplayController.GetPhysicalMonitors(_currentMonitor);
        private static MMDevice? _playbackDevice;
        public static void Init()
        {
            try
            {
                _playbackDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            }
            catch(Exception ex)
            {
                ClientLibrary.Log.Default.Error(ex, "Init");
            }
        }
        public static void ScreenPowerOff() => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
        public static void ScreenPowerOn() => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
        public static void SendKeys(Keys keys) => Keybd_Event(keys, 0, 0, 0);
        public static void SetVolume(int volume)
        {
            try
            {
                if (_playbackDevice?.AudioEndpointVolume != null)
                {
                    volume = volume > 100 ? 100 : volume;
                    volume = volume < 0 ? 0 : volume;
                    _playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = volume / 100f;
                }
            }
            catch(Exception ex)
            {
                App.ShowMessage(ex.Message, new TimeSpan(0, 0, 5));
            }
        }
        public static int GetVolume()
        {
            if (_playbackDevice?.AudioEndpointVolume != null)
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
                    App.ShowMessage(ex.Message,new TimeSpan(0,0,5));
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
            if (Environment.OSVersion.Version.Major < 10)
            {
                FakePowerOff(date); return;
            }
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
        public static async void FakePowerOff(DateTime date)
        {
            ScreenPowerOff();
            var ts = date.TimeOfDay - DateTime.Now.TimeOfDay;
            await Task.Delay((int)ts.Duration().TotalMilliseconds);
            Reboot();
        }
        public static void Reboot()
        {
            Process.Start("shutdown.exe", "-r -f -t 0");
        }
        public static void ShutDown()
        {
            Process.Start("shutdown.exe", "-s -f -t 0");
        }
        public static void SaveConfiguration(string name, string value)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = configFile.AppSettings.Settings;
            var json = JsonSerializer.Serialize(value);
            if (setting[name] == null)
                setting.Add(name, value);
            else
                setting[name].Value = value;
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        public static string LoadConfiguration(string name)
        {
            var value = ConfigurationManager.AppSettings[name];
            return !string.IsNullOrEmpty(value) ? value : "";
        }
        public static void SetAutoLogin()
        {
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;
            if (ver.Major == 6)
            {
                RegistryKey key = Registry.LocalMachine;
                var winlogon = key.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
                winlogon?.SetValue("AutoAdminLogon", 1);
                winlogon?.Close();
            }
        }
        public static void SetAutoBoot()
        {
            var startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),"client.lnk");
            var shellType = Type.GetTypeFromProgID("WScript.Shell");
            dynamic? shell = Activator.CreateInstance(shellType!);
            var shortcut = shell?.CreateShortcut(startupPath);
            if (shortcut != null)
            {
                shortcut.TargetPath = Process.GetCurrentProcess().MainModule?.FileName;
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                shortcut.Save();
            }
        }
        public static bool SetDate(DateTime date)
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
        public static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == (int)WM_DEVICECHANGE)
            {
                USBUpdate();
            }
            return IntPtr.Zero;
        }
        public static void HideLauncher()
        {
            IntPtr intPtr = FindWindow("ConsoleWindowClass", "ClientLauncher");
            if (intPtr != IntPtr.Zero)
                ShowWindow(intPtr, 0);
        }
        private static void USBUpdate()
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {
                var path = System.IO.Path.Combine(drive.RootDirectory.FullName, "update");
                if(System.IO.Directory.Exists(path))
                    foreach (var item in System.IO.Directory.GetFiles(path))
                    {
                        if (item.Contains("update.json"))
                        {
                            Guard.Stop();
                            AutoUpdater.ParseUpdateInfoEvent += o =>
                            {
                                var info = JsonSerializer.Deserialize<AutoUpdaterDotNET.UpdateInfoEventArgs>(o.RemoteData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (info != null)
                                    info.DownloadURL = System.IO.Path.Combine(path, "client.zip");
                                o.UpdateInfo = info;
                            };
                            AutoUpdater.Start(item);
                        }
                    }
            }
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
