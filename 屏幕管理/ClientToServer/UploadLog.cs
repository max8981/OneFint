namespace 屏幕管理.ClientToServer
{
    internal class UploadLog:ClientTopic
    {
        public UploadLog(string code):base(code) { }
        [JsonPropertyName("time_stamp")]
        public long TimeStamp => (System.DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        [JsonPropertyName("log")]
        public string? Log { get; set; }
    }
}
