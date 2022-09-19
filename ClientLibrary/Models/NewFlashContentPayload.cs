using System;

namespace ClientLibrary.Models
{
    public class NewFlashContentPayload
    {
        [JsonPropertyName("new_flash_content")]
        public Content NewFlashContent { get; set; }

        /// <summary>
        /// 循环次数，与end_at互斥
        /// </summary>
        [JsonPropertyName("loop_time")]
        public int? LoopTime { get; set; }

        /// <summary>
        /// 结束时间，与loop_time互斥
        /// </summary>
        [JsonPropertyName("end_at")]
        public string? EndAt { get; set; }
    }
}

