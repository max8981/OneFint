using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal struct Device
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("org")]
        public Org Org { get; set; }
        [JsonPropertyName("mac_address")]
        public string? MacAddress { get; set; }
        [JsonPropertyName("started_at")]
        public string? Started_at { get; set; }
        [JsonPropertyName("ended_at")]
        public string? Ended_at { get; set; }
    }
}
