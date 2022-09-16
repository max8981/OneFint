using System;
using ClientLibrary.Models;

namespace ClientLibrary.Models
{
    public class Layout:BaseDateAt
	{
        /// <summary>
        /// 布局ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 布局名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 布局宽度
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }
        /// <summary>
        /// 布局高度
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }
        [JsonPropertyName("content")]
        public LayoutContent Content { get; set; }
    }
}

