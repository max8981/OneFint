using System;
namespace SharedProject
{
	public class EmergencyContent:BaseContent
	{
        /// <summary>
        /// 紧急内容
        /// </summary>
        [JsonPropertyName("emergency_contents")]
        public Content[] EmergencyContents { get; set; }
        public static EmergencyContent FromJson(string json)
        {
            return FromJson<EmergencyContent>(json);
        }
    }
}

