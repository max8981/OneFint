namespace ClientLibrary.ServerToClient
{
    /// <summary>
    /// 定时启动
    /// </summary>
    public partial class TimingBoot : Topic
    {
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public Models.TimingBootPolicy[]? Policies { get; set; }
    }
}