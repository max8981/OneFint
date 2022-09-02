using System;
namespace SharedProject
{
	public class NewFlashContent:BaseContent
	{
        /// <summary>
        /// 插播内容
        /// </summary>
        [JsonPropertyName("new_flash_content_payloads")]
        public NewFlashContentPayload[] NewFlashContentPayloads { get; set; }
        public static NewFlashContent FromJson(string json)
        {
            return FromJson<NewFlashContent>(json);
        }
    }
}

