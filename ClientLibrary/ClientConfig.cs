using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class ClientConfig
    {
        public ClientConfig(IClient client)
        {
            _client = client;
            Code = "";
            MqttServer = "47.101.178.160";
            MqttPort = 1883;
            MqttUser = "admin";
            MqttPassword = "public";
            HeartBeatSecond = 10;
            MaterialPath = "./materials";
            UpdateUrl = "http://yuxtech.com:20000/api/Update/GetVersion?key=c1";
        }
        private readonly IClient _client;
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
        public void Save()
        {
            foreach (var item in GetType().GetProperties())
            {
                var name = item.Name;
                var value = item.GetValue(this);
                if (value != null)
                    _client.SaveConfiguration(name, value.ToString()!);
            }
        }
        public ClientConfig Load()
        {
            Code = _client.LoadConfiguration(nameof(Code));
            MqttServer= _client.LoadConfiguration(nameof(MqttServer));
            if (int.TryParse(_client.LoadConfiguration(nameof(MqttPort)), out var port))
                MqttPort = port;
            MqttUser= _client.LoadConfiguration(nameof(MqttUser));
            MqttPassword=_client.LoadConfiguration(nameof(MqttPassword));
            HeartBeatSecond = int.TryParse(_client.LoadConfiguration(nameof(HeartBeatSecond)), out var number) ? number : 10;
            MaterialPath = _client.LoadConfiguration(nameof(MaterialPath));
            DelayedUpdate = !bool.TryParse(_client.LoadConfiguration(nameof(DelayedUpdate)), out var delayeUpdate) || delayeUpdate;
            ShowDownloader = !bool.TryParse(_client.LoadConfiguration(nameof(ShowDownloader)), out var showDownloader) || showDownloader;
            UpdateUrl = _client.LoadConfiguration(nameof(UpdateUrl));
            return this;
        }
        
    }
}
