namespace 屏幕管理.Models
{
    public partial class Policy : BaseDateAt
    {
        /// <summary>
        /// 策略ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 设备ID
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }
        /// <summary>
        /// 设备组ID
        /// </summary>
        [JsonPropertyName("device_group_id")]
        public int DeviceGroupId { get; set; }
        /// <summary>
        /// 在loop_type为1或2时，此字段的时间为当天日期+开机时间
        /// </summary>
        [JsonPropertyName("started_at")]
        public string? StartedAt { get; set; }
        /// <summary>
        /// 在loop_type为1或2时，此字段的时间为当天日期+关机时间
        /// </summary>
        [JsonPropertyName("ended_at")]
        public string? EndedAt { get; set; }
    }
}

