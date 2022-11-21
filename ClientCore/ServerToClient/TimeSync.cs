using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ServerToClient
{
    internal class TimeSync:ServerTopic
    {
        [JsonPropertyName("forwar_second")]
        public int ForwardSecond { get; set; }
    }
}
