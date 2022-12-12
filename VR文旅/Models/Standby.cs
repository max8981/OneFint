using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal class Standby
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("resource_url")]
        public string? ResourceUrl { get; set; }
        [JsonPropertyName("mute")]
        public bool Mute { get; set; }
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        [JsonPropertyName("started_at")]
        public string? StartedAt { get; set; }
        [JsonPropertyName("ended_at")]
        public string? EndedAt { get; set; }
    }
}
