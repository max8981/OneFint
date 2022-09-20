namespace ClientLibrary.Models
{
    /// <summary>
    /// 设备
    /// </summary>
    public partial class Device:BaseDateAt
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        /// <summary>
        /// 设备组
        /// </summary>
        [JsonPropertyName("device_group")]
        public DeviceGroup? DeviceGroup { get; set; }
        /// <summary>
        /// 分辨率X
        /// </summary>
        [JsonPropertyName("resolution_x")]
        public int ResolutionX { get; set; }
        /// <summary>
        /// 分辨率Y
        /// </summary>
        [JsonPropertyName("resolution_y")]
        public int ResolutionY { get; set; }
        /// <summary>
        /// 所在地区
        /// </summary>
        [JsonPropertyName("location")]
        public string? Location { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        [JsonPropertyName("address")]
        public string? Address { get; set; }
        [JsonPropertyName("owner")]
        public Owner? Owner { get; set; }
        /// <summary>
        /// 布局
        /// </summary>
        [JsonPropertyName("layout")]
        public Layout? Layout { get; set; }
        /// <summary>
        /// 硬件厂商
        /// </summary>
        [JsonPropertyName("hardware")]
        public Hardware? Hardware { get; set; }
        /// <summary>
        /// 设备预览
        /// </summary>
        [JsonPropertyName("preview_url")]
        public string? PreviewUrl { get; set; }
        /// <summary>
        /// 设备状态：0：UNKNOWN_DEVICE_STATUS；1：ONLINE；2：VIDEO；3：TEXT
        /// </summary>
        [JsonPropertyName("status")]
        public Enums.DeviceStatusEnum Status { get; set; }
        /// <summary>
        /// 当前设备正在播放的内容数
        /// </summary>
        [JsonPropertyName("playing_content_num")]
        public int PlayingContentNum { get; set; }
        /// <summary>
        /// 设备信息
        /// </summary>
        [JsonPropertyName("machine_info")]
        public MachineInfo? MachineInfo { get; set; }
    }
}

