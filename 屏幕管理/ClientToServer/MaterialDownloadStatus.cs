namespace 屏幕管理.ClientToServer
{
    internal class MaterialDownloadStatus:IClientTopic
    {
        public MaterialDownloadStatus(int contentId, bool downloaded, int? deviceId, int? deviceGroupId)
        {
            ContentId = contentId;
            Downloaded = downloaded;
            DeviceId = deviceId;
            DeviceGroupId = deviceGroupId;
        }
        [JsonPropertyName("content_id")]
        public int ContentId { get; set; }
        [JsonPropertyName("downloaded")]
        public bool Downloaded { get; set; }
        [JsonPropertyName("device_id")]
        public int? DeviceId { get; set; }
        [JsonPropertyName("device_group_id")]
        public int? DeviceGroupId { get; set; }
    }
}
