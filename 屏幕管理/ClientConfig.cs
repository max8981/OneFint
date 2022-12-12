using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理
{
    internal class ClientConfig
    {
        public ClientConfig()
        {
            Load();
        }
        public string Code { get; set; }
        public string MqttServer { get; set; }
        public int MqttPort { get; set; }
        public string MqttUser { get; set; }
        public string MqttPassword { get; set; }
        public string UpdateUrl { get; set; }
        public int HeartBeatSecond { get; set; }
        public string MaterialPath { get; set; }
        public bool DelayedUpdate { get; set; }
        public bool ShowDownloader { get; set; }
        public bool AutoReboot { get; set; }
        public int GuardInterval { get; set; }
        public void Save()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var setting = configFile.AppSettings.Settings;
            foreach (var item in GetType().GetProperties())
            {
                var name = item.Name;
                var value = item.GetValue(this);
                if (setting[name] == null)
                    setting.Add(name,value?.ToString());
                else
                    setting[name].Value = value?.ToString();
            }
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }
        public void Load()
        {
            foreach (var item in GetType().GetProperties())
            {
                var value = ConfigurationManager.AppSettings[item.Name];

                if (value != null)
                    switch (item.PropertyType.Name)
                    {
                        case "Int32":
                            item.SetValue(this, int.Parse(value));
                            break;
                        case "Boolean":
                            item.SetValue(this, bool.Parse(value));
                            break;
                        default:
                            item.SetValue(this, value);
                            break;
                    }
            }
        }
    }
}
