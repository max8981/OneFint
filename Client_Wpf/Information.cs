using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Client_Wpf
{
    public class Information
    {
        public static string MachineName => Environment.MachineName;
        public static string MachineCode => GetMachineCode();
        public static string OSVersion => Environment.OSVersion.VersionString;
        public static string DiskSize => GetDiskSize();
        public static string DesktopMonitor => GetDesktopMonitor();
        public static string MacAddress => GetMacByNetworkInterface();
        public static string IpAddress => GetByManagementClass().First().ToString();
        public static int Row => GetRow();
        private static long GetMemorySize()
        {
            return 0;
        }
        private static int GetRow()
        {
            var result = 0;
            ManagementClass hardDisk = new(WindowsAPIs.WindowsApiTypeEnum.Win32_DiskDrive.ToString());
            ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
            foreach (var item in hardDiskC)
            {
                long capacity = Convert.ToInt64(item[WindowsAPIs.WindowsApiKeyEnum.Size.ToString()].ToString());
                result += (int)capacity / 1000_000_000;
            }
            return result;
        }
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
        private static string GetMacByNetworkInterface()
        {
            try
            {
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    return BitConverter.ToString(ni.GetPhysicalAddress().GetAddressBytes());
                }
            }
            catch (Exception)
            {
            }
            return "00-00-00-00-00-00";
        }
        private static List<IPAddress> GetByManagementClass()
        {
            try
            {
                ManagementClass mClass = new System.Management.ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection managementObjectCollection = mClass.GetInstances();
                List<IPAddress> ls = new List<IPAddress>();
                foreach (var item in managementObjectCollection)
                {
                    if ((bool)item["IPEnabled"] == true)
                    {
                        foreach (var ip in (string[])item["IPAddress"])
                        {
                            if(IPAddress.TryParse(ip, out var ipout))
                                ls.Add(ipout);
                        }
                    }
                }
                return ls;
            }
            catch (Exception)
            {
                return new List<IPAddress>();

            }
        }
        private static string GetMachineCode()
        {
            var mac = GetMacByNetworkInterface();
            var arr = mac.Split("-");
            return string.Join("", arr);
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
        private static string Increment(int num)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var numList = new List<char>();
            while (true)
            {
                int remainder = num % chars.Length;
                numList.Add(chars[remainder]);
                num /= chars.Length;

                if (num != 0) continue;

                numList.Reverse();
                return new string(numList.ToArray());
            }
        }
    }
}
