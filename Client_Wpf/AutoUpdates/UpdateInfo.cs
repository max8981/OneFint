using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Client_Wpf.AutoUpdates
{
    public record UpdateInfo
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("changelog")]
        public string Changelog { get; set; }
        [JsonPropertyName("mandatory")]
        public Mandatory Mandatory { get; set; }
        [JsonPropertyName("checksum")]
        public Checksum Checksum { get; set; }
    }
    public record Mandatory
    {
        [JsonPropertyName("value")]
        public bool Value { get; set; }
        [JsonPropertyName("minVersion")]
        public string MinVersion { get; set; }
        [JsonPropertyName("mode")]
        public int Mode { get; set; }
    }
    public record Checksum
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("hashingAlgorithm")]
        public string HashingAlgorithm { get; set; }
    }
}
