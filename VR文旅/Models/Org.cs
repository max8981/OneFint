using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal struct Org
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("org_name")]
        public string Org_name { get; set; }
        [JsonPropertyName("province")]
        public string Province { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
