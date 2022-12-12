namespace 屏幕管理.ServerToClient
{
    internal class ClientControl
    {
        [JsonPropertyName("order")]
        public Models.Order? Order { get; set; }
    }
}
