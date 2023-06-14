using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace 屏幕管理.Systems
{
    internal class Config
    {
        private static readonly Type _config = typeof(Config);
        public static void SetTest()
        {
            Code = Global.MacAddress.Replace("-", "");
            MqttServer = "172.19.10.205";
            MqttPort = 24200;
            MqttUser = "admin";
            MqttPassword = "public";
            UpdateUrl = "localhost";
            MaterialPath = "materials";
            HeartBeatSecond = 50;
        }
        public static void SetTest2()
        {
            Code = Global.MacAddress.Replace("-", "");
            MqttServer = "172.19.10.205";
            MqttPort = 34756;
            MqttUser = "admin";
            MqttPassword = "public";
            UpdateUrl = "localhost";
            MaterialPath = "materials";
            HeartBeatSecond = 50;
        }
        public static string Code { get; private set; } = Network.GetMacAddress().Replace("-", "");
        public static string MqttServer { get; set; } = "47.101.178.160";
        public static int MqttPort { get; set; } = 1883;
        public static string MqttUser { get; set; } = "admin";
        public static string MqttPassword { get; set; } = "publice";
        public static string? UpdateUrl { get; set; }
        public static int HeartBeatSecond { get; set; } = 50;
        public static string MaterialPath { get => Controllers.DownloadController.GetMaterialPath(); set => Controllers.DownloadController.SetMaterialsPath(value); }
        public static bool AutoReboot { get; set; }
        public static int RebootTimeHours { get; set; } = 3;
        public static int GuardInterval { get; set; } = 3000;
        public static string? Dns { get; set; }
        public static bool FakeShutdown { get; set; } = true;
        public static bool ShowDownload { get;set; }
        public static bool OfflinePlay { get; set; }
        public static bool DelayedRefresh { get; set; }
        public static int DeviceId { get; set; }
        public static int DeviceGroupId { get; set; }
        public static void Save()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = configFile.AppSettings.Settings;
            foreach (var item in _config.GetProperties())
            {
                if (item != null&&item.CanRead)
                {
                    var name = item.Name;
                    var value = item.GetValue(_config);
                    if (setting[name] == null)
                        setting.Add(name, value?.ToString());
                    else
                        setting[name].Value = value?.ToString();
                }
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        public static void Load()
        {
            foreach (var item in _config.GetProperties())
            {
                var value = ConfigurationManager.AppSettings[item.Name];
                if (value != null&&item.CanWrite)
                    switch (item.PropertyType.Name)
                    {
                        case "Int32":
                            item.SetValue(_config, int.TryParse(value, out var result) ? result : 0);
                            break;
                        case "Boolean":
                            item.SetValue(_config, bool.Parse(value));
                            break;
                        default:
                            item.SetValue(_config, value);
                            break;
                    }
            }
        }
        private static string[]? GetNetworkParameter(string value)
        {
            var list = new List<string>();
            foreach (var item in value.Split(','))
                if (IsIPAddress(item))
                    list.Add(item);
            return list.Count > 0 ? list.ToArray() : null;
        }
        private static bool IsIPAddress(string ip)
        {
            //将完整的IP以“.”为界限分组
            string[] arr = ip.Split('.');


            //判断IP是否为四组数组成
            if (arr.Length != 4)
                return false;


            //正则表达式，1~3位整数
            string pattern = @"\d{1,3}";
            for (int i = 0; i < arr.Length; i++)
            {
                string d = arr[i];


                //判断IP开头是否为0
                if (i == 0 && d == "0")
                    return false;


                //判断IP是否是由1~3位数组成
                if (!Regex.IsMatch(d, pattern))
                    return false;

                if (d != "0")
                {
                    //判断IP的每组数是否全为0
                    d = d.TrimStart('0');
                    if (d == "")
                        return false;

                    //判断IP每组数是否大于255
                    if (int.Parse(d) > 255)
                        return false;
                }
            }
            return true;
        }
    }
}
