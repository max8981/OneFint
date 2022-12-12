namespace 屏幕管理.ServerToClient
{
    public class ScreenControl:ServerTopic
    {
        [JsonPropertyName("screen_activation")]
        public bool ScreenActivation { get; set; }
    }
}
