using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            Code = "max01";
            Server = "ws://47.101.178.160:8083/mqtt";
            MqttUser = "admin";
            MqttPassword = "public";
            HeartBeatSecond = 10;
            MaterialPath = "./materials";
        }
        public string Code { get; set; }
        public string Server { get; set; }
        public string MqttUser { get; set; }
        public string MqttPassword { get; set; }
        public int HeartBeatSecond { get; set; }
        public string MaterialPath { get; set; }
        public bool DelayedUpdate { get; set; }
    }
}
