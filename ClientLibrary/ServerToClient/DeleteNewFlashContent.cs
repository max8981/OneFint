namespace ClientLibrary.ServerToClient
{
    public partial class DeleteNewFlashContent:Topic
    {
        /// <summary>
        /// 待删除的插播内容
        /// </summary>
        [JsonPropertyName("new_flash_contents")]
        public Models.Content[]? NewFlashContents { get; set; }
    }
}