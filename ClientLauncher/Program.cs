// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.IO;


int time = 30;

System.Timers.Timer _timer = new System.Timers.Timer(30 * 1000);
string appPath = "./Client_Wpf.exe";
_timer.AutoReset = true;
_timer.Elapsed += (o, e) =>
{
    var ps = Process.GetProcessesByName("Client_Wpf");
    if (!ps.Any())
    {
        Process.Start(appPath);
    }
};
if (System.IO.File.Exists(appPath))
{
    _timer.Start();
}
SpinWait.SpinUntil(()=>false);