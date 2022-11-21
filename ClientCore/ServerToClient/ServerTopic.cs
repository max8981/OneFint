global using System.Text.Json.Serialization;
global using System.Text.Json;

namespace ClientCore.ServerToClient
{
    public class ServerTopic
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
