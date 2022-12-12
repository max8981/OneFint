namespace 屏幕管理.ServerToClient
{
    public partial class DeleteNewFlashContent:ServerTopic
    {
        /// <summary>
        /// 待删除的插播内容
        /// </summary>
        [JsonPropertyName("new_flash_contents")]
        public Models.Content[]? NewFlashContents { get; set; }
    }
}