namespace 屏幕管理.ClientToServer
{
    internal class ClientTopic
    {
        public ClientTopic(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        internal string Code { get; set; }
    }
}
