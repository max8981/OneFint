using System;
namespace SharedProject
{
	public class NormalContent:BaseContent
	{
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
        public static NormalContent FromJson(string json) => JsonSerializer.Deserialize<NormalContent>(json);
    }
}

