global using System.Text.Json.Serialization;
global using System.Text.Json;

namespace ClientLibrary.ServerToClient
{
    public class Topic
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
