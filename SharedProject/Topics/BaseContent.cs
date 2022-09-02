using System;
namespace SharedProject
{
	public class BaseContent:GlobalUsings
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
    }
}

