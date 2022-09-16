using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ServerToClient
{
    public class Topic
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
