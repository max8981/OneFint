using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MiniLog
{
    internal class Log
    {
        private readonly LogConfiguration _configuration;
        public Log(LogConfiguration configuration) {
            _configuration = configuration;
        }
        public void Info(string title, string message,params string[] others) {

        }
        public void Info(LogData data, [CallerMemberName]string memberName="")
        {
            data.Title = memberName;
            data.Level = Enums.LogLevelEnum.Info;
        }
        public void Warn(string message) { }
        public void Error(string message) { }
        public void Fatal(string message) { }
        public void Trace(string message, 
            [CallerMemberName]string memberName = "", 
            [CallerFilePath]string filePath="",
            [CallerLineNumber]int lineNumber=0) { }
    }
}
