using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理.Systems
{
    internal class Memory
    {
        private static long GetHardDiskSize()
        {
            long result = 0;
            System.Management.ManagementClass hardDisk = new("Win32_DiskDrive");
            System.Management.ManagementObjectCollection hardDiskC = hardDisk.GetInstances();
            foreach (var item in hardDiskC)
            {
                long capacity = Convert.ToInt64(item["Size"].ToString());
                result += capacity;
            }
            return result;
        }
        private static long GetPhisicalMemory()
        {
            ManagementObjectSearcher searcher = new()
            {
                Query = new SelectQuery("Win32_PhysicalMemory", "", new string[] { "Capacity" })
            };
            ManagementObjectCollection collection = searcher.Get();
            ManagementObjectCollection.ManagementObjectEnumerator em = collection.GetEnumerator();
            long result = 0;
            while (em.MoveNext())
            {
                ManagementBaseObject baseObj = em.Current;
                var value = baseObj.Properties["Capacity"].Value;
                if (long.TryParse(value.ToString(), out var capacity))
                    result += capacity;
            }
            return result;
        }
        private static long GetHardDiskSpace()
        {
            long result = 0;
            foreach (var drive in System.IO.DriveInfo.GetDrives())
                result += drive.TotalFreeSpace;
            return result;
        }
        private static long GetRootPathSize()
        {
            long result = 0;
            foreach (var drive in System.IO.DriveInfo.GetDrives())
                if (drive.Name == Path.GetPathRoot(AppContext.BaseDirectory))
                    result = drive.TotalSize;
            return result;
        }
        private static long GetRootPathSpace()
        {
            long result = 0;
            foreach (var drive in System.IO.DriveInfo.GetDrives())
                if (drive.Name == Path.GetPathRoot(AppContext.BaseDirectory))
                    result = drive.TotalFreeSpace;
            return result;
        }
        internal static long HardDiskSize => GetHardDiskSize();
        internal static long HardDiskSpace => GetHardDiskSpace();
        internal static long RootPathSize => GetRootPathSize();
        internal static long RootPathSpace => GetRootPathSpace();
        internal static long PhisicalMemory => GetPhisicalMemory();
    }
}
