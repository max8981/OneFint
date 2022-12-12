using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理.ClientToServer
{
    internal class HeartBeat:ClientTopic
    {
        public HeartBeat(string code):base(code) { }
    }
}
