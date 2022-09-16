using System;
using SharedProject;

namespace ClientLibrary.ServerToClient
{
    /// <summary>
    /// title
    /// </summary>
    public partial class TimingVolume : Topic
    {
        /// <summary>
        /// 启动策略列表
        /// </summary>
        [JsonPropertyName("policies")]
        public TimingVolumePolicy[] Policies { get; set; }
    }
}

