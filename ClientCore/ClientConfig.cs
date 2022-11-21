using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            Code = "";
            MqttServer = "47.101.178.160";
            MqttPort = 1883;
            MqttUser = "admin";
            MqttPassword = "public";
            HeartBeatSecond = 10;
            MaterialPath = "./materials";
            UpdateUrl = "http://yuxtech.com:20000/api/Update/GetVersion?key=c1";
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
    }
}
