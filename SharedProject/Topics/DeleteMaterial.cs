using System;
namespace SharedProject
{
	public class DeleteMaterial:GlobalUsings
	{
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("device_id")]
        public decimal? DeviceId { get; set; }

        /// <summary>
        /// 是否删除所有素材
        /// </summary>
        [JsonPropertyName("delete_all")]
        public bool DeleteAll { get; set; }

        [JsonPropertyName("materials")]
        public Material[] Materials { get; set; }
        public static DeleteMaterial FromJson(string json)
        {
            return FromJson<DeleteMaterial>(json);
        }
    }
}

