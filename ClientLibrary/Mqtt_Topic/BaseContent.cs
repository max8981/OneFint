using System;
namespace ClientLibrary
{
	public class BaseContent
	{
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
        /// <summary>
        /// 布局
        /// </summary>
        [JsonPropertyName("layout")]
        public Layout Layout { get; set; }
        /// <summary>
        /// 普通内容
        /// </summary>
        [JsonPropertyName("normal_contents")]
        public Content[] NormalContents { get; set; }
        /// <summary>
        /// 托底内容
        /// </summary>
        [JsonPropertyName("default_contents")]
        public Content[] DefaultContents { get; set; }
        /// <summary>
        /// 紧急内容
        /// </summary>
        [JsonPropertyName("emergency_contents")]
        public Content[] EmergencyContents { get; set; }
        /// <summary>
        /// 插播内容
        /// </summary>
        [JsonPropertyName("new_flash_content_payloads")]
        public NewFlashContentPayload[] NewFlashContentPayloads { get; set; }
    }
}

