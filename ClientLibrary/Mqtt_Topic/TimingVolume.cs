using System;
namespace SharedProject
{
    /// <summary>
    /// title
    /// </summary>
    public partial class TimingVolume:GlobalUsings
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
        public TimingVolumePolicy[] Policies { get; set; }
        public static TimingVolume FromJson(string json)
        {
            return FromJson<TimingVolume>(json);
        }
    }
}

