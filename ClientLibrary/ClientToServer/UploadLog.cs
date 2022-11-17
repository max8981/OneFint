using ClientLibrary.ServerToClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ClientToServer
{
    internal class UploadLog:Topic
    {
        public long TimeStamp => ClientHelper.TimeStamp;
        public string? Log { get; set; }
    }
}
