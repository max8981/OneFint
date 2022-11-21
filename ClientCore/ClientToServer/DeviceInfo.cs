using ClientLibrary.ServerToClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ClientToServer
{
    internal class DeviceInfo:ClientTopic
    {
        [JsonPropertyName("time_stamp")]
        public long TimeStamp => ClientHelper.TimeStamp;
        [JsonPropertyName("screen_activation")]
        public bool ScreenActivation { get; set; }
        [JsonPropertyName("fefresh_content")]
        public bool RefreshContent { get; set; }
        [JsonPropertyName("mac_addr")]
        public string? MacAddress { get; set; }
        [JsonPropertyName("rom")]
        public int Rom { get; set; }
        [JsonPropertyName("free_disk")]
        public int FreeDisk { get; set; }
        [JsonPropertyName("ram")]
        public int Ram { get; set; }
        [JsonPropertyName("system_type")]
        public Enums.SystemTypeEnum SystemType { get; set; }
        [JsonPropertyName("app_version")]
        public string? AppVersion { get; set; }
        [JsonPropertyName("ip_addr")]
        public string? IpAddress { get; set; }
    }
}
