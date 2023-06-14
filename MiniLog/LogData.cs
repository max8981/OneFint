using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniLog
{
    internal struct LogData
    {
        private static readonly System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions()
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString | System.Text.Json.Serialization.JsonNumberHandling.WriteAsString,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
        };
        public LogData(string message,params object[] args)
        {
            Message = message;
            Others = args.Select(x=>System.Text.Json.JsonSerializer.Serialize(x, options)).ToArray();
        }
        public string Title = "Log";
        public Enums.LogLevelEnum Level = Enums.LogLevelEnum.None;
        public string Message;
        public string[] Others;
    }
}
