using System;

namespace ClientLibrary.Models
{
    public class Page
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// UUID唯一
        /// </summary>
        [JsonPropertyName("uuid")]
        public string Uuid { get; set; }
        /// <summary>
        /// 页面名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("components")]
        public Component[] Components { get; set; }
        /// <summary>
        /// 是否跳转
        /// </summary>
        [JsonPropertyName("skip")]
        public bool Skip { get; set; }
        /// <summary>
        /// 跳转时间
        /// </summary>
        [JsonPropertyName("skip_second")]
        public int SkipSecond { get; set; }
        /// <summary>
        /// 跳转至
        /// </summary>
        [JsonPropertyName("skip_to")]
        public string SkipTo { get; set; }
    }
}

