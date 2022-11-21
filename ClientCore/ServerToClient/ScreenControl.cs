using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCore.ServerToClient
{
    public class ScreenControl:ServerTopic
    {
        [JsonPropertyName("screen_activation")]
        public bool ScreenActivation { get; set; }
    }
}
