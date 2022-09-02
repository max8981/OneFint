namespace SharedProject
{
    public partial class Policy
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        /// <summary>
        /// 设备组ID
        /// </summary>
        [JsonPropertyName("device_group_id")]
        public int? DeviceGroupId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        [JsonPropertyName("device_id")]
        public int? DeviceId { get; set; }

        /// <summary>
        /// 在loop_type为1或2时，此字段的时间为当天日期+关机时间
        /// </summary>
        [JsonPropertyName("ended_at")]
        public string EndedAt { get; set; }

        /// <summary>
        /// 策略ID
        /// </summary>
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        /// <summary>
        /// 循环类型，可选0，1，2，3; 0:UNKNOWN_BOOT_POLICY_LOOP_TYPE; 1:EVERYDAY（每天）; 2:SOMEDAY（每周几）;
        /// 3:PREIOD（某时段，含日期）
        /// </summary>
        [JsonPropertyName("loop_type")]
        public int? LoopType { get; set; }

        /// <summary>
        /// 在loop_type为1或2时，此字段的时间为当天日期+开机时间
        /// </summary>
        [JsonPropertyName("started_at")]
        public string StartedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}

