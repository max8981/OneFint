using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ClientConfig : ClientCore.IClientConfig
    {
        public string Code { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string MqttServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MqttPort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string MqttUser { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string MqttPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string UpdateUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int HeartBeatSecond { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string MaterialPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool DelayedUpdate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowDownloader { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoReboot { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int GuardInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
