using System.Diagnostics;
using System.Reflection;

namespace ClientGuard
{
    internal class Program
    {
        private static readonly System.Timers.Timer _timer = new(30 * 1000);
        private static readonly string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static void Main(string[] args)
        {
            int interval = 30;
            string appPath = "";
            int processId = 0;
            var name = "";
            foreach (var item in args)
            {
                var p = item.Split('=');
                if (p.Length == 2)
                {
                    switch (p[0].ToUpper())
                    {
                        case "A":
                            appPath = p[1];
                            break;
                        case "I":
                            interval = int.TryParse(p[1], out var time) ? time : 10;
                            break;
                        case "N":
                            name = p[1];
                            break;
                        case "P":
                            processId = int.TryParse(p[1], out var pid) ? pid : -1;
                            break;
                    }
                }
            }
            _timer.AutoReset = true;
            _timer.Elapsed += (o, e) =>
            {
                var ps = Process.GetProcessesByName(name);
                if (!ps.Any())
                {
                    Process.Start(appPath);
                }
            };
            if (System.IO.File.Exists(appPath))
            {
                _timer.Interval = interval * 1000;
                _timer.Start();
            }
            else
            {
                return;
            }
            SpinWait.SpinUntil(() => false);
        }
    }
}