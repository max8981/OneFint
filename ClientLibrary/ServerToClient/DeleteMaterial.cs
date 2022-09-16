using System;
using ClientLibrary.Models;

namespace ClientLibrary.ServerToClient
{
    public class DeleteMaterial:Topic
    {
        [JsonPropertyName("device_id")]
        public decimal? DeviceId { get; set; }

        /// <summary>
        /// 是否删除所有素材
        /// </summary>
        [JsonPropertyName("delete_all")]
        public bool DeleteAll { get; set; }

        [JsonPropertyName("materials")]
        public Material[] Materials { get; set; }
    }
}

