using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using 屏幕管理.ClientToServer;

namespace 屏幕管理.Systems
{
    public class Log
    {
        public const string InfoLevel = "INFO";
        public const string WarnLevel = "WARN";
        public const string ErrorLevel = "ERROR";
        private readonly ConcurrentQueue<string[]> logMsgQueue = new();
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
                    if (logMsgQueue.Count >= 10 || !logMsgQueue.IsEmpty && (DateTime.Now - lastSaveLogTime).TotalSeconds > 30)//如是待写日志消息累计>=10条或上一次距离现在写日志时间超过30s则需要批量提交日志
                    {
                        List<string[]> logMsgList = new();
                        while (logMsgList.Count < 10 && logMsgQueue.TryDequeue(out var logMsgItems))
                            logMsgList.Add(logMsgItems);

                        WriteLog(logMsgList);

                        lastSaveLogTime = DateTime.Now;
                    }
                    else SpinWait.SpinUntil(() => logMsgQueue.Count >= 10, 5000);
                }
            }, cancellationToken);
        }
        private string GetLogFilePath()
        {
            string logFileDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logFileDir))
                Directory.CreateDirectory(logFileDir);
            string logDateStr = DateTime.Now.ToString("yyyyMMdd");
            string logName = logDateStr;
            if (!string.IsNullOrEmpty(todayLogName) && todayLogName.StartsWith(logName))
                logName = todayLogName;
            else todayLogName = logName;

            string logFilePath = Path.Combine(logFileDir, logName + ".log");

            if (File.Exists(logFilePath))
            {
                File.SetAttributes(logFilePath, FileAttributes.Normal);
                if (File.GetLastWriteTime(logFilePath).Month != DateTime.Today.Month) //30天滚动(删除旧的文件)，防止日志文件过多
                {
                    File.Delete(logFilePath);
                    string[] oldLogFiles = Directory.GetFiles(logFileDir, string.Format("{0}-##.log", logDateStr), SearchOption.TopDirectoryOnly);
                    foreach (string fileName in oldLogFiles)
                    {
                        File.SetAttributes(fileName, FileAttributes.Normal);
                        File.Delete(fileName);
                    }
                }
                else if (new FileInfo(logFilePath).Length > MaxSizeBackup)
                {
                    Regex rgx = new Regex(@"^\d{8}-(?<fnum>\d{2})$");
                    int fnum = 2;
                    if (rgx.IsMatch(logName))
                        fnum = int.Parse(rgx.Match(logName).Groups["fnum"].Value) + 1;
                    UpLoadLog(todayLogName);
                    logName = string.Format("{0}-{1:D2}", logDateStr, fnum);
                    todayLogName = logName;
                    logFilePath = Path.Combine(logFileDir, logName + ".log");
                }
            }

            return logFilePath;
        }
        private void WriteLog(IEnumerable<string[]> logMsgs)
        {
            try
            {
                List<string> logMsgLines = new List<string>();
                foreach (var logMsgItems in logMsgs)
                {
                    var logMsgLineFields = (new object[] { DateTime.Now }).Concat(logMsgItems).ToArray();
                    string logMsgLineText = string.Format(LineLayoutRenderFormat, logMsgLineFields);
                    logMsgLines.Add(logMsgLineText);
                }

                string logFilePath = GetLogFilePath();
                File.AppendAllLines(logFilePath, logMsgLines);
            }
            catch
            { }
        }
        public static Log Default => instance;
        public static string LogFilePath => Default.GetLogFilePath();
        public Action<string> UpLoadLog = logPath => { };
        public string LineLayoutRenderFormat
        {
            get => lineLayoutRenderFormat;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("无效的LineLayoutRenderFormat属性值");
                lineLayoutRenderFormat = value;
            }
        }
        public long MaxSizeBackup
        {
            get => maxSizeBackup;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("无效的MaxSizeBackup属性值");
                maxSizeBackup = value;
            }
        }
        public void SaveLog(string logLevel, string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            logMsgQueue.Enqueue(new[] { logLevel, Environment.CurrentManagedThreadId.ToString(), source, msg, str1 ?? string.Empty, str2 ?? string.Empty, str3 ?? string.Empty, str4 ?? string.Empty });
        }
        public void Info(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(InfoLevel, msg, source, str1, str2, str3, str4);
        }
        public void Warn(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(WarnLevel, msg, source, str1, str2, str3, str4);
        }
        public void Error(string msg, string source, string? str1 = null, string? str2 = null, string? str3 = null, string? str4 = null)
        {
            SaveLog(ErrorLevel, msg, source, str1, str2, str3, str4);
        }
        public void Error(Exception ex, string source, string? other1 = null, string? other2 = null, string? other3 = null)
        {
            SaveLog(ErrorLevel, ex.Message, source, ex.StackTrace, other1, other2, other3);
        }
        public void Close() => cts.Cancel();
        ~Log()
        {
            cts.Cancel();
        }

    }
}
