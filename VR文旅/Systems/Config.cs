using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VR文旅.Systems
{
    internal class Config
    {
        private static readonly Type _config = typeof(Config);
        public static string MacAddress { get; set; } = "";
        public static int OrdId { get; private set; } = 1;
        public static string Server { get;private set; } = "http://47.101.178.160:8079";
        public static string PublicKey { get; private set; } = "";
        public static int WaitTime { get; private set; } = 15 * 60;
        public static string UpdateUrl { get; private set; } = "";
        public static double ZoomLevel { get; private set; } = 2.0;
        public static void Save()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = configFile.AppSettings.Settings;
            foreach (var item in _config.GetProperties())
            {
                if (item != null && item.CanRead)
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
                if (value != null && item.CanWrite)
                    switch (item.PropertyType.Name)
                    {
                        case "Int32":
                            item.SetValue(_config, int.Parse(value));
                            break;
                        case "Double":
                            item.SetValue(_config, double.Parse(value));
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
    }
}
