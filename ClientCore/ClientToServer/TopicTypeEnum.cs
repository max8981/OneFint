using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ClientToServer
{
    internal enum TopicTypeEnum
    {
        heartbeat,
        material_download_status,
        reconnect,

        device_info,
        upload_log,
    }
}
