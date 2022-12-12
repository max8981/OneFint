namespace 屏幕管理.Models
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class MachineInfo:BaseDateAt
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }
        [JsonPropertyName("device_group_id")]
        public int DeviceGroupId { get; set; }
        /// <summary>
        /// 设备音量
        /// </summary>
        [JsonPropertyName("volume")]
        public int Volume { get; set; }
        /// <summary>
        /// 设备组音量
        /// </summary>
        [JsonPropertyName("device_group_volume")]
        public int DeviceGroupVolume { get; set; }
        /// <summary>
        /// MAC地址
        /// </summary>
        [JsonPropertyName("mac")]
        public string? Mac { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        [JsonPropertyName("ip")]
        public string? Ip { get; set; }
        /// <summary>
        /// 存储空间
        /// </summary>
        [JsonPropertyName("rom")]
        public string? Rom { get; set; }
        /// <summary>
        /// 运行内存
        /// </summary>
        [JsonPropertyName("ram")]
        public string? Ram { get; set; }
        /// <summary>
        /// 系统类型
        /// </summary>
        [JsonPropertyName("os_type")]
        public string? OsType { get; set; }
        /// <summary>
        /// 系统版本
        /// </summary>
        [JsonPropertyName("os_version")]
        public string? OsVersion { get; set; }
        /// <summary>
        /// 以秒为单位的在线时长
        /// </summary>
        [JsonPropertyName("alive")]
        public long Alive { get; set; }
        /// <summary>
        /// 上次清理时间
        /// </summary>
        [JsonPropertyName("cleared_at")]
        public string? ClearedAt { get; set; }
        /// <summary>
        /// 最后上线时间
        /// </summary>
        [JsonPropertyName("last_online_at")]
        public string? LastOnlineAt { get; set; }
    }
}

