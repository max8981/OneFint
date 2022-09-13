namespace SharedProject
{
    /// <summary>
    /// 定时启动
    /// </summary>
    public partial class TimingBoot:GlobalUsings
    {
        /// <summary>
        /// 机器码 uuid
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public TimingBootPolicy[] Policies { get; set; }
        public static TimingBoot FromJson(string json)
        {
            return FromJson<TimingBoot>(json);
        }
    }
}