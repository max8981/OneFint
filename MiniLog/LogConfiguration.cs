using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLog
{
    internal class LogConfiguration
    {
        public LogConfiguration(string name, Enums.LogTypeEnum type, string context)
        {
            LogName = name;
            LogType = type;
            LogContext = context;
        }
        public string LogName { get; set; }
        public string? LogDescription { get; set; }
        public Enums.LogTypeEnum LogType { get; set; }
        public string LogContext { get; set; }
    }
}
