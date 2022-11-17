using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client_Wpf
{
    class Guard
    {
        private static readonly System.Timers.Timer _timer = new(3 * 1000);
        private static readonly string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Client_Wpf.exe");
        private static readonly string _appPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ClientGuard.exe");
        private static readonly string _name = "ClientGuard";
        public static void Start(int time=30)
        {
            _timer.AutoReset = true;
            _timer.Elapsed += (o, e) =>
            {
                var ps = Process.GetProcessesByName(_name);
                if (!ps.Any())
                {
                    ClientLibrary.Log.Default.Warn("启动守护", "Guard");
                    Process.Start(_appPath, $"a={path} i={time} n=Client_Wpf");
                }
            };
            if (System.IO.File.Exists(_appPath))
            {
                ClientLibrary.Log.Default.Warn("开始监视守护", "Guard");
                _timer.Start();
            }
        }
        public static void Stop()
        {
            _timer.Stop();
            var ps = Process.GetProcessesByName(_name);
            foreach (var item in ps)
            {
                item.Kill();
            }
        }
    }
}
