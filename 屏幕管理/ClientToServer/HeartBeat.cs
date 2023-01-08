using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理.ClientToServer
{
    internal class HeartBeat:IClientTopic
    {
        public HeartBeat(string code)
        {
            Code = code;
        }
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
