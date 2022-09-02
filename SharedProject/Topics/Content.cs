using System;
namespace SharedProject
{
	public class Content:BaseDateAt
	{
        /// <summary>
        /// 内容ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }
        /// <summary>
        /// 内容名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        [JsonPropertyName("device")]
        public Device Device { get; set; }
        /// <summary>
        /// 设备组
        /// </summary>
        [JsonPropertyName("device_group")]
        public DeviceGroup DeviceGroup { get; set; }
        [JsonPropertyName("component")]
        public Component Component { get; set; }
        /// <summary>
        /// 网址
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        [JsonPropertyName("text")]
        public BaseText Text { get; set; }
        /// <summary>
        /// 素材
        /// </summary>
        [JsonPropertyName("material")]
        public Material Material { get; set; }
        /// <summary>
        /// 内容状态；0：UNKNOWN_CONTENT_STATUS；1：TO_REVIEW；2：REVIEW_FAIL；3：PLAYING；4：TO_PLAY; 5: EXPIRED
        /// </summary>
        [JsonPropertyName("status")]
        public StatusEnum Status { get; set; }
        /// <summary>
        /// 内容类型：0：UNKNOWN_CONTENT_TYPE；1:NORMAL; 2: EMERGENCY; 3: NEWFLASH; 4:DEFAULT
        /// </summary>
        [JsonPropertyName("content_type")]
        public ContentTypeEnum ContentType { get; set; }
        /// <summary>
        /// 是否校验
        /// </summary>
        [JsonPropertyName("verified")]
        public bool Verified { get; set; }
        /// <summary>
        /// 校验时间
        /// </summary>
        [JsonPropertyName("verified_at")]
        public string VerifiedAt { get; set; }
        /// <summary>
        /// 开始投放时间
        /// </summary>
        [JsonPropertyName("started_at")]
        public string StartedAt { get; set; }
        /// <summary>
        /// 投放结束时间
        /// </summary>
        [JsonPropertyName("ended_at")]
        public string EndedAt { get; set; }
        /// <summary>
        /// 播放时间
        /// </summary>
        [JsonPropertyName("play_duration")]
        public int PlayDuration { get; set; }
        /// <summary>
        /// 是否静音播放
        /// </summary>
        [JsonPropertyName("mute")]
        public bool Mute { get; set; }
        /// <summary>
        /// 是否已下载
        /// </summary>
        [JsonPropertyName("downloaded")]
        public bool Downloaded { get; set; }
        /// <summary>
        /// 在content_type相同时，content的次序
        /// </summary>
        [JsonPropertyName("order")]
        public int Order { get; set; }

        public enum ContentTypeEnum
        {
            UNKNOWN_CONTENT_TYPE,
            NORMAL,
            EMERGENCY,
            NEWFLASH,
            DEFAULT,
        }
        public enum StatusEnum
        {
            UNKNOWN_CONTENT_STATUS,
            TO_REVIEW,
            REVIEW_FAIL,
            PLAYING,
            TO_PLAY,
            EXPIRED,
        }
    }
}

