namespace 屏幕管理.ServerToClient
{
    /// <summary>
    /// 定时启动
    /// </summary>
    public partial class TimingBoot : ServerTopic
    {
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public Models.TimingBootPolicy[]? Policies { get; set; }
    }
}