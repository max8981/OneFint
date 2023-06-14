using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static VR文旅.Log;

namespace VR文旅
{
    public class Log
    {
        public const string InfoLevel = "INFO";
        public const string WarnLevel = "WARN";
        public const string ErrorLevel = "ERROR";
        public string LogFilePath => GetLogFilePath();
        public static bool IsHtmLog { get; set; }
        //private readonly ConcurrentQueue<string[]> logMsgQueue = new();
        private readonly ConcurrentQueue<LogContent> logMsgQueue = new();
        private readonly CancellationTokenSource cts = null!;
        private string lineLayoutRenderFormat = "[{0:yyyy-MM-dd HH:mm:ss}]\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}；";
        private long maxSizeBackup = 10485760L;//默认10MB
        private string? todayLogName = null;
        private static readonly Log instance = new();
        private Log()
        {
            cts = new CancellationTokenSource();
            ListenSaveLogAsync(cts.Token);
        }
        private void ListenSaveLogAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                DateTime lastSaveLogTime = DateTime.Now;
                while (!cancellationToken.IsCancellationRequested)//如果没有取消线程，则一直监听执行写LOG
                {
                    if (logMsgQueue.Count >= 10 || (!logMsgQueue.IsEmpty && (DateTime.Now - lastSaveLogTime).TotalSeconds > 30))//如是待写日志消息累计>=10条或上一次距离现在写日志时间超过30s则需要批量提交日志
                    {
                        List<LogContent> logMsgList = new();
                        while (logMsgList.Count < 10 && logMsgQueue.TryDequeue(out var logMsgItems))
                        {
                            logMsgList.Add(logMsgItems);
                        }
                        if (IsHtmLog)
                            WriteHtmlLog(logMsgList);
                        else
                            WriteLog(logMsgList);
                        lastSaveLogTime = DateTime.Now;
                    }
                    else
                    {
                        SpinWait.SpinUntil(() => logMsgQueue.Count >= 10, 5000);//自旋等待直到日志队列有>=10的记录或超时5S后再进入下一轮的判断
                    }
                }
            }, cancellationToken);
        }
        private string GetLogFilePath()
        {
            string logFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string ext = IsHtmLog ? ".htm" : ".log";
            if (!Directory.Exists(logFileDir))
            {
                Directory.CreateDirectory(logFileDir);
            }

            string logDateStr = DateTime.Now.ToString("yyyyMMdd");
            string logName = logDateStr;
            if (!string.IsNullOrEmpty(todayLogName) && todayLogName.StartsWith(logName))
            {
                logName = todayLogName;
            }
            else
            {
                todayLogName = logName;
            }

            string logFilePath = Path.Combine(logFileDir, logName + ext);

            if (File.Exists(logFilePath))
            {
                File.SetAttributes(logFilePath, FileAttributes.Normal);
                if (File.GetLastWriteTime(logFilePath).Month != DateTime.Today.Month) //30天滚动(删除旧的文件)，防止日志文件过多
                {
                    File.Delete(logFilePath);
                    string[] oldLogFiles = Directory.GetFiles(logFileDir, string.Format("{0}-##{1}", logDateStr, ext), SearchOption.TopDirectoryOnly);
                    foreach (string fileName in oldLogFiles)
                    {
                        File.SetAttributes(fileName, FileAttributes.Normal);
                        File.Delete(fileName);
                    }
                }
                else if (new FileInfo(logFilePath).Length > MaxSizeBackup)
                {
                    Regex rgx = new(@"^\d{8}-(?<fnum>\d{2})$");
                    int fnum = 2;
                    if (rgx.IsMatch(logName))
                    {
                        fnum = int.Parse(rgx.Match(logName).Groups["fnum"].Value) + 1;
                    }

                    logName = string.Format("{0}-{1:D2}", logDateStr, fnum);
                    todayLogName = logName;
                    logFilePath = Path.Combine(logFileDir, logName + ext);
                }
            }

            return logFilePath;
        }
        private void WriteLog(IEnumerable<LogContent> logContents)
        {
            try
            {
                List<string> logMsgLines = new();
                foreach (var logMsgItems in logContents)
                {
                    var logMsgLineFields = (new object[] { 
                        logMsgItems.LogLeve.ToString(), 
                        logMsgItems.ThreadId.ToString(), 
                        logMsgItems.Date, 
                        logMsgItems.Source, 
                        string.Join(';', logMsgItems.Messages), 
                        string.Join(',', logMsgItems.Params) });
                    string logMsgLineText = string.Format(LineLayoutRenderFormat, logMsgLineFields);
                    logMsgLines.Add(logMsgLineText);
                }

                string logFilePath = GetLogFilePath();
                File.AppendAllLines(logFilePath, logMsgLines);
            }
            catch
            { }
        }
        private void WriteHtmlLog(IEnumerable<LogContent> logContents)
        {
            try
            {
                List<string> logMsgLines = new();
                foreach (var content in logContents)
                {
                    var head = "<HR COLOR=red>";
                    var date = $"异常事件：{content.Date:yyyy-MM-dd HH:mm:ss,FFF} [{content.ThreadId}]<BR>";
                    var leve = $"异常级别：{content.LogLeve}<BR>";
                    var source = $"异常来源：{content.Source}<BR>";
                    var paramas = string.Join(',', content.Params) + "<BR>";
                    var messages = string.Join(';', content.Messages) + "<BR>";
                    var line = "<HR Size=1>";
                    logMsgLines.Add(head);
                    logMsgLines.Add(date);
                    logMsgLines.Add(leve);
                    logMsgLines.Add(source);
                    logMsgLines.Add(paramas);
                    logMsgLines.Add(messages);
                    logMsgLines.Add(line);
                    logMsgLines.Add(line);
                }

                string logFilePath = GetLogFilePath();
                File.AppendAllLines(logFilePath, logMsgLines);
            }
            finally { }
        }
        public string GetLogBase64()
        {
            using FileStream filestream = new(GetLogFilePath(), FileMode.Open);
            byte[] bt = new byte[filestream.Length];
            filestream.Read(bt, 0, bt.Length);
            var base64Str = Convert.ToBase64String(bt);
            return base64Str;
        }
        public static Log Default => instance;
        public string LineLayoutRenderFormat
        {
            get { return lineLayoutRenderFormat; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("无效的LineLayoutRenderFormat属性值");
                }

                lineLayoutRenderFormat = value;
            }
        }
        public long MaxSizeBackup
        {
            get { return maxSizeBackup; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("无效的MaxSizeBackup属性值");
                }

                maxSizeBackup = value;
            }
        }
        public void SaveLog(LogLeve logLevel, string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            //logMsgQueue.Enqueue(new[] { logLevel.ToString(), Environment.CurrentManagedThreadId.ToString(), source, msg, str1 ?? string.Empty, str2 ?? string.Empty, str3 ?? string.Empty, str4 ?? string.Empty });
            logMsgQueue.Enqueue(new LogContent(source, logLevel, new string[] { msg, str1, str2, str3, str4 }));
        }
        public void Info(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(LogLeve.Info, msg, source, str1, str2, str3, str4);
        }
        public void Warn(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(LogLeve.Warning, msg, source, str1, str2, str3, str4);
        }
        public void Error(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(LogLeve.Error, msg, source, str1, str2, str3, str4);
        }
        public void Error(Exception ex, string source, string? other1 = null, string? other2 = null, string? other3 = null)
        {
            SaveLog(LogLeve.Error, ex.Message, source, ex.StackTrace, other1, other2, other3);
        }
        ~Log()
        {
            cts.Cancel();
        }
        public struct LogContent
        {
            public LogContent(string source,LogLeve logLeve,string[] messages,params string[] strings)
            {
                ThreadId = Environment.CurrentManagedThreadId;
                Source = source;
                Messages = messages;
                Params = strings;
                LogLeve = logLeve;
                Date = DateTime.Now;
            }
            public int ThreadId { get; }
            public string Source { get; }
            public DateTime Date { get; }
            public LogLeve LogLeve { get; }
            public object[] Params { get; }
            public string[] Messages { get; set; }
        }
        public enum LogLeve
        {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
        }
    }
}
