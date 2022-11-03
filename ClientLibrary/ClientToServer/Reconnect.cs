using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ClientToServer
{
    internal class Reconnect:HeartBeat
    {
        public Reconnect(string code)
        {
            Code = code;
            MaterialResend = true;
        }
        [JsonPropertyName("material_resend")]
        public bool MaterialResend { get; set; }
    }
}
