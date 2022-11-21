using System;

namespace ClientCore.Models
{
    public class BaseDateAt
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }
    }
}

