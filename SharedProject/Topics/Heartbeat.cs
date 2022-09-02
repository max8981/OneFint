using System;
namespace SharedProject
{
	public class Heartbeat
	{
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
	}
}

