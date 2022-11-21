namespace ClientCore.ServerToClient
{
    public partial class TimingVolume : ServerTopic
    {
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public Models.TimingVolumePolicy[]? Policies { get; set; }
    }
}

