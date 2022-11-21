using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ServerToClient
{
    internal class ClientControl
    {
        [JsonPropertyName("order")]
        public Models.Order? Order { get; set; }
    }
}
