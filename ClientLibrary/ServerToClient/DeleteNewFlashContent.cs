using System.Text.Json.Serialization;
using ClientLibrary.Models;

namespace ClientLibrary.ServerToClient
{
    public partial class DeleteNewFlashContent:Topic
    {
        /// <summary>
        /// 待删除的插播内容
        /// </summary>
        [JsonPropertyName("new_flash_contents")]
        public Content[] NewFlashContents { get; set; }
    }
}