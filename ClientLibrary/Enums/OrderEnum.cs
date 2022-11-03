using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.Enums
{
    internal enum OrderEnum
    {
        UNKNOWN_CLIENT_ORDER,
        SHUTDOWN,
        REBOOT,
        SCREEN_BOOT,
        SCREEN_SHUTDOWN,
        CLEAN_UP_CACHE,
        VOLUME_CONTROL,
    }
}
