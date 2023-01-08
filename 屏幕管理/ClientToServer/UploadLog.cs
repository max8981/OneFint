namespace 屏幕管理.ClientToServer
{
    internal class UploadLog:IClientTopic
    {
        public UploadLog(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("time_stamp")]
        public long TimeStamp => (System.DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        [JsonPropertyName("log")]
        public string? Log { get; set; }
    }
}
