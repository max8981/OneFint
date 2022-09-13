using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class ClientConfig
    {
        public string Code { get; set; } = "max01";
        public string Server { get; set; } = "ws://47.101.178.160:8083/mqtt";
        public string MqttUser { get; set; } = "admin";
        public string MqttPassword { get; set; } = "public";
        public int HeartBeatSecond { get; set; } = 10;
    }
}
