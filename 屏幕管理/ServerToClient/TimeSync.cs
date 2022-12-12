namespace 屏幕管理.ServerToClient
{
    internal class TimeSync:ServerTopic
    {
        [JsonPropertyName("forwar_second")]
        public int ForwardSecond { get; set; }
    }
}
