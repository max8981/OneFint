using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ClientToServer
{
    internal class UploadLog:ClientTopic
    {
        public long TimeStamp => (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        public string? Log { get; set; }
    }
}
