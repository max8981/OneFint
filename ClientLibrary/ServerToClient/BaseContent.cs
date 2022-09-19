using System;
using ClientLibrary.Models;

namespace ClientLibrary.ServerToClient
{
    public class BaseContent:Topic
    {
        /// <summary>
        /// 布局
        /// </summary>
        [JsonPropertyName("layout")]
        public Layout Layout { get; set; }
        /// <summary>
        /// 普通内容
        /// </summary>
        [JsonPropertyName("normal_contents")]
        public Content[]? NormalContents { get; set; }
        /// <summary>
        /// 托底内容
        /// </summary>
        [JsonPropertyName("default_contents")]
        public Content[]? DefaultContents { get; set; }
        /// <summary>
        /// 紧急内容
        /// </summary>
        [JsonPropertyName("emergency_contents")]
        public Content[]? EmergencyContents { get; set; }
        /// <summary>
        /// 插播内容
        /// </summary>
        [JsonPropertyName("new_flash_content_payloads")]
        public NewFlashContentPayload[]? NewFlashContentPayloads { get; set; }
    }
}

