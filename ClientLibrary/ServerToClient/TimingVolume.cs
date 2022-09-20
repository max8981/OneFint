namespace ClientLibrary.ServerToClient
{
    public partial class TimingVolume : Topic
    {
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public Models.TimingVolumePolicy[]? Policies { get; set; }
    }
}

