using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Client_Wpf
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
                var size = GetByteString(capacity, 1000);
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
        public static string GetByteString(double value, int mod)
        {
            char[] units = new char[] { '\0', 'K', 'M', 'G', 'T', 'P' };
            int i = 0;
            while (mod < value)
            {
                value /= mod;
                i++;
            }
            return $"{Math.Round(value)}{units[i]}b";
        }
    }
}
