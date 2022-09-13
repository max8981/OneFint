using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class Information
    {
        public string MachineName => Environment.MachineName;
        public string OSVersion => Environment.OSVersion.VersionString;
        public string DiskSize => GetDiskSize();
        public string DesktopMonitor => GetDesktopMonitor();
        private static string GetDiskSize()
        {
            string hdId = string.Empty;
            var result = string.Empty;
            ManagementClass hardDisk = new(WindowsAPIs.WindowsApiTypeEnum.Win32_DiskDrive.ToString());
            ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
            foreach (var item in hardDiskC)
            {
                long capacity = Convert.ToInt64(item[WindowsAPIs.WindowsApiKeyEnum.Size.ToString()].ToString());
                var size = ClientHelper.GetByteString(capacity, 1000);
                var name = item["Caption"].ToString();
                result += $"{name}\t{size}";
            }
            return result;
        }
        private static string GetDesktopMonitor()
        {
            var result = string.Empty;
            ManagementClass mc = new(WindowsAPIs.WindowsApiTypeEnum.Win32_DesktopMonitor.ToString());
            foreach (var item in mc.GetInstances())
            {
                foreach (var p in item.Properties)
                {
                    Console.WriteLine($"{p.Name}:{p.Value}");
                }
                result = $"{item[WindowsAPIs.WindowsApiKeyEnum.ScreenWidth.ToString()]}*{item[WindowsAPIs.WindowsApiKeyEnum.ScreenHeight.ToString()]}";
                break;
            }
            return result;
        }
    }
}
