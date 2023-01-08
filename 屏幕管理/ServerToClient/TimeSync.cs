namespace 屏幕管理.ServerToClient
{
    internal class TimeSync:ServerTopic
    {
        [JsonPropertyName("forward_second")]
        public long ForwardSecond { get; set; }
    }
}
