namespace 屏幕管理.ServerToClient
{
    public class DeleteMaterial:ServerTopic
    {
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// 是否删除所有素材
        /// </summary>
        [JsonPropertyName("delete_all")]
        public bool DeleteAll { get; set; }

        [JsonPropertyName("materials")]
        public Models.Material[]? Materials { get; set; }
    }
}

