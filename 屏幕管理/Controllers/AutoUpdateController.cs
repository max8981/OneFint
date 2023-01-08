using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 屏幕管理.Systems;

namespace 屏幕管理.Controllers
{
    internal class AutoUpdateController
    {
        private const uint WM_DEVICECHANGE = 0x0219;
        internal static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg == (int)WM_DEVICECHANGE)
                USBUpdate();
            return IntPtr.Zero;
        }
        internal static void USBUpdate()
        {
            DriveInfo[] s = DriveInfo.GetDrives();
            foreach (DriveInfo drive in s)
            {
                var path = System.IO.Path.Combine(drive.RootDirectory.FullName, "update");
                if (System.IO.Directory.Exists(path))
                    foreach (var item in System.IO.Directory.GetFiles(path))
                    {
                        if (item.Contains("update.json"))
                        {
                            //Guard.Stop();
                            AutoUpdater.ParseUpdateInfoEvent += o =>
                            {
                                var info = JsonSerializer.Deserialize<AutoUpdaterDotNET.UpdateInfoEventArgs>(o.RemoteData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (info != null)
                                    info.DownloadURL = System.IO.Path.Combine(path, "client.zip");
                                o.UpdateInfo = info;
                            };
                            CheckUpdate(item);
                            return;
                        }
                    }
            }
        }
        internal static void NetUpdate(string? updateUrl)
        {
            AutoUpdater.InstalledVersion = new Version(Global.AppVersion);
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.ParseUpdateInfoEvent += o =>
            {
                try
                {
                    var info = JsonSerializer.Deserialize<UpdateInfoEventArgs>(o.RemoteData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    o.UpdateInfo = info;
                    //Guard.Stop();
                }
                catch(Exception ex)
                {
                    Log.Default.Error(ex, "NetUpdate");
                }

            };
            CheckUpdate(updateUrl);
        }
        internal static void CheckUpdate(string? updateUrl)
        {
            if (updateUrl != null)
                AutoUpdater.Start(updateUrl);
        }
    }
}
