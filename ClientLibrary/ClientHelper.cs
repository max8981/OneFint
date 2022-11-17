using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    internal class ClientHelper
    {
        internal static long TimeStamp => (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;
        private static long GetTimeStamp()
        {
            TimeSpan timeSpan = DateTime.Now - new DateTime();
            return Convert.ToInt64(timeSpan.TotalSeconds);
        }
    }
}
