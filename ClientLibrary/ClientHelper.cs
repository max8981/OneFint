using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    internal static class ClientHelper
    {
        internal static long TimeStamp => (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        private static int _forwardSecond;
        private static long GetTimeStamp()
        {
            TimeSpan timeSpan = DateTime.Now - new DateTime();
            return Convert.ToInt64(timeSpan.TotalSeconds);
        }
        internal static void SetforwardSecond(this int forwardSecond) => _forwardSecond = forwardSecond;
        internal static DateTime Now=> DateTime.UtcNow.AddSeconds(_forwardSecond);
    }
}
