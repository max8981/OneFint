using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ServerToClient
{
    public class ScreenControl:Topic
    {
        [JsonPropertyName("screen_activation")]
        public bool ScreenActivation { get; set; }
    }
}
