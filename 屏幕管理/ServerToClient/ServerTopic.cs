namespace 屏幕管理.ServerToClient
{
    public class ServerTopic
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
