using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理
{
    internal interface IClientConfig
    {
        string Code { get; set; }
        string MqttServer { get; set; }
        int MqttPort { get; set; }
        string MqttUser { get; set; }
        string MqttPassword { get; set; }
        string UpdateUrl { get; set; }
        int HeartBeatSecond { get; set; }
        string MaterialPath { get; set; }
        bool DelayedUpdate { get; set; }
        bool ShowDownloader { get; set; }
        bool AutoReboot { get; set; }
        int GuardInterval { get; set; }        
    }
}
