using System.Text.Json.Serialization;
namespace SharedProject
{
    public class MaterialDownloadUrl:GlobalUsings
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// 内容id
        /// </summary>
        [JsonPropertyName("content_id")]
        public int ContentId { get; set; }

        /// <summary>
        /// 与device_group_id互斥，二选一
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// 与device_id互斥，二选一
        /// </summary>
        [JsonPropertyName("device_group_id")]
        public int DeviceGroupId { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        public static MaterialDownloadUrl FromJson(string json)
        {
            return FromJson<MaterialDownloadUrl>(json);
        }
    }
}