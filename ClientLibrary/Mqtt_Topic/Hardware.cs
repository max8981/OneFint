using System;
namespace ClientLibrary
{
    /// <summary>
    /// 硬件厂商
    /// </summary>
    public class Hardware
    {
        /// <summary>
        /// 硬件厂商ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 硬件厂商名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}

