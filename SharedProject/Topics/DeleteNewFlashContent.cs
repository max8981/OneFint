﻿using System.Text.Json.Serialization;

namespace SharedProject
{
    public partial class DeleteNewFlashContent:GlobalUsings
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 待删除的插播内容
        /// </summary>
        [JsonPropertyName("new_flash_contents")]
        public Content[] NewFlashContents { get; set; }
        public static DeleteNewFlashContent FromJson(string json)
        {
            return FromJson<DeleteNewFlashContent>(json);
        }
    }
}