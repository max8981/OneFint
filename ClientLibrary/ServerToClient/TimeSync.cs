﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.ServerToClient
{
    internal class TimeSync:Topic
    {
        [JsonPropertyName("forwar_second")]
        public int ForwardSecond { get; set; }
    }
}
