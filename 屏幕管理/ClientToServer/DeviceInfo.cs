namespace 屏幕管理.ClientToServer
{
    internal class DeviceInfo:IClientTopic
    {
        public DeviceInfo(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("time_stamp")]
        public long TimeStamp => (System.DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        [JsonPropertyName("screen_activation")]
        public bool ScreenActivation { get; set; }
        [JsonPropertyName("refresh_content")]
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
