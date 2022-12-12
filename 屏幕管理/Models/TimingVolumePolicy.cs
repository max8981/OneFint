namespace 屏幕管理.Models
{
    public class TimingVolumePolicy : Policy
    {
        /// <summary>
        /// 音量，范围为0-15
        /// </summary>
        [JsonPropertyName("volume")]
        public int Volume { get; set; }
    }
}

