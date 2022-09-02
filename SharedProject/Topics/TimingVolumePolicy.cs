using System;
namespace SharedProject
{
	public class TimingVolumePolicy:Policy
	{
        /// <summary>
        /// 音量，范围为0-15
        /// </summary>
        [JsonPropertyName("volume")]
        public int? Volume { get; set; }
    }
}

