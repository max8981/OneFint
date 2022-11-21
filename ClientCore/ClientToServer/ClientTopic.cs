using System;
using System.Collections.Generic;
using System.Text;

namespace ClientCore.ClientToServer
{
    internal class ClientTopic
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        [JsonPropertyName("code")]
        internal string? Code { get; set; }
    }
}
