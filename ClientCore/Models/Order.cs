using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.Models
{
    internal class Order
    {
        /// <summary>
        /// 与device_group_id互斥，二选一
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// 与device_id互斥，二选一
        /// </summary>
        [JsonPropertyName("device_group_id")]
        public int DeviceGroupId { get; set; }
        [JsonPropertyName("order")]
        public Enums.OrderEnum OrderEnum { get; set; }
        [JsonPropertyName("volume")]
        public int Volume { get; set; }
        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
