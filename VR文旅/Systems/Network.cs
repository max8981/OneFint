using CefSharp.DevTools.LayerTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using VR文旅;

namespace VR文旅.Systems
{
    internal class Network
    {
        private static readonly ManagementClass _wmi = new("Win32_NetworkAdapterConfiguration");
        internal static void SetIPAddress(string[]? ip, string[]? submask, string[]? getway, string[]? dns)
        {
            //_ = wmi.GetInstances();
            foreach (ManagementObject mo in _wmi.GetInstances().Cast<ManagementObject>())
            {
                //如果没有启用IP设置的网络设备则跳过
                if (!(bool)mo["IPEnabled"])
                    continue;

                ManagementBaseObject? inPar;
                //设置IP地址和掩码
                if (ip != null && submask != null)
                {
                    inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = ip;
                    inPar["SubnetMask"] = submask;
                    _ = mo.InvokeMethod("EnableStatic", inPar, null!);
                }

                //设置网关地址
                if (getway != null)
                {
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = getway;
                    _ = mo.InvokeMethod("SetGateways", inPar, null!);
                }

                //设置DNS地址
                if (dns != null)
                {
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inPar["DNSServerSearchOrder"] = dns;
                    _ = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null!);
                }
            }
        }
        internal static string GetIPAddresses()
        {
            var result = "0.0.0.0";
            try
            {
                foreach (var item in _wmi.GetInstances())
                    if ((bool)item["IPEnabled"] == true)
                        foreach (var ip in (string[])item["IPAddress"])
                            if (System.Net.IPAddress.TryParse(ip, out var ipout))
                                result = ipout.ToString();
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "GetMacAddress");
            }
            return result;
        }
        internal static string GetMacAddress()
        {
            var result = "00-00-00-00-00-00";
            try
            {
                var interfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                var address = interfaces.First().GetPhysicalAddress().GetAddressBytes();
                result = BitConverter.ToString(address);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "GetMacAddress");
            }
            return result;
        }
        internal static void SetDns(string dns) => SetIPAddress(null, null, null, GetNetworkParameter(dns));
        private static string[]? GetNetworkParameter(string value)
        {
            var list = new List<string>();
            foreach (var item in value.Split(','))
                if (IPAddress.TryParse(item, out _))
                    list.Add(item);
            return list.Count > 0 ? list.ToArray() : null;
        }
    }
}
