using System;
namespace SharedProject
{
    /// <summary>
    /// 设备组
    /// </summary>
    public class DeviceGroup
    {
        /// <summary>
        /// 设备组ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 设备组name
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 设备总数
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
        /// <summary>
        /// 在线数
        /// </summary>
        [JsonPropertyName("online")]
        public int Online { get; set; }
        /// <summary>
        /// 离线数
        /// </summary>
        [JsonPropertyName("offline")]
        public int Offline { get; set; }
        [JsonPropertyName("devices")]
        public object Devices { get; set; }
        /// <summary>
        /// 布局
        /// </summary>
        [JsonPropertyName("layout")]
        public Layout Layout { get; set; }
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}

