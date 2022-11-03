using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ClientToServer
{
    public class HeartBeat
    {
        public HeartBeat(string code)
        {
            Code = code;
        }
        public HeartBeat() { }

        [JsonPropertyName("code")]
        public string Code { get; set; }
    }
}
