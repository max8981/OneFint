using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VR文旅.Models
{
    internal struct Location
    {
        public Location(string province, string city)
        {
            Province = province;
            City = city;
        }
        [JsonPropertyName("province")]
        public string Province { get; set; }
        [JsonPropertyName("city")]
        public string City { get; set; }
    }
}
