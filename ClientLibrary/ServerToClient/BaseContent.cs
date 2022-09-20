namespace ClientLibrary.ServerToClient
{
    public class BaseContent:Topic
    {
        /// <summary>
        /// 布局
        /// </summary>
        [JsonPropertyName("layout")]
        public Models.Layout? Layout { get; set; }
        /// <summary>
        /// 普通内容
        /// </summary>
        [JsonPropertyName("normal_contents")]
        public Models.Content[]? NormalContents { get; set; }
        /// <summary>
        /// 托底内容
        /// </summary>
        [JsonPropertyName("default_contents")]
        public Models.Content[]? DefaultContents { get; set; }
        /// <summary>
        /// 紧急内容
        /// </summary>
        [JsonPropertyName("emergency_contents")]
        public Models.Content[]? EmergencyContents { get; set; }
        /// <summary>
        /// 插播内容
        /// </summary>
        [JsonPropertyName("new_flash_content_payloads")]
        public Models.NewFlashContentPayload[]? NewFlashContentPayloads { get; set; }
    }
}

