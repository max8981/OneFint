using Cjwdev.WindowsApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ClientService
{
    public partial class Guard : ServiceBase
    {
        public Guard()
        {
            InitializeComponent();
        }
        private const string LOG_PATH = "c:/log";
        private readonly System.Timers.Timer _timer = new System.Timers.Timer(30 * 1000);
        protected override void OnStart(string[] args)
        {
            string path = "c:/ClientLauncher.ini";
            string appPath = string.Empty;
            if (System.IO.File.Exists(path))
                appPath = System.IO.File.ReadAllText(path);
            WriteLog("OnStart", "服务启动");
            _timer.AutoReset = true;
            _timer.Elapsed += (o, e) =>
            {
                var ps = Process.GetProcessesByName("Client_Wpf");
                for (int i = 0; i < ps.Length; i++)
                {
                    WriteLog("Process", $"{ps[i].Id}");
                }
                if (!ps.Any())
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        WriteLog("Elapsed", "无启动配置文件");
                        return;
                    }
                    if (!System.IO.File.Exists(appPath))
                    {
                        WriteLog("Elapsed", "启动程序路径错误");
                        return;
                    }
                    WriteLog("Elapsed", "程序未运行，重启启动");
                    try
                    {
                        Launcher(appPath);
                    }
                    catch(Exception ex)
                    {
                        WriteLog("Launcher", ex.Message);

                    }
                }
            };
            _timer.Start();
        }
        private void WriteLog(string title,string content)
        {
            if (!System.IO.Directory.Exists(LOG_PATH))
                System.IO.Directory.CreateDirectory(LOG_PATH);
            System.IO.File.AppendAllText(System.IO.Path.Combine(LOG_PATH, $"{DateTime.Now:yyyy-MM-dd}.txt"), $"{DateTime.Now:F}\t{title}\t{content}\r\n");
        }
        protected override void OnStop()
        {
        }
        private void Launcher(string path)
        {
            var appPath = path;
            IntPtr userTokenHandle = IntPtr.Zero;
            ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);
            ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
            ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
            startInfo.cb = (uint)Marshal.SizeOf(startInfo);
            ApiDefinitions.CreateProcessAsUser(userTokenHandle, appPath, "", IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startInfo, out procInfo);
            if (userTokenHandle != IntPtr.Zero)
                ApiDefinitions.CloseHandle(userTokenHandle);
            int _currentAquariusProcessId = (int)procInfo.dwProcessId;
        }
    }
}
