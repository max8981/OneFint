using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public record DownloadTask
    {
        public DownloadTask(int id,string url,string? name=null,string? path=null)
        {
            Id = id;
            Uri = new Uri(url??"");
            FileName = name?.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 ? name : System.Web.HttpUtility.UrlDecode(Uri.Segments.Last());
            Ext = Path.GetExtension(FileName);
            FilePath = path?.IndexOfAny(Path.GetInvalidPathChars()) < 0 ? path : "./materials"; ;
            CreatDate = DateTime.Now;
            if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
        }
        public int Id { get; init; }
        public string FileName { get; init; }
        public string Ext { get; init; }
        public string FilePath { get; init; }
        public FileInfo FileInfo => new($"{FilePath}/{FileName}");
        public Uri Uri { get; init; }
        public string Source => FileInfo.FullName;
        public bool IsExists => File.Exists(FileInfo.FullName);
        public long TotalSize { get; private set; }
        public long DownloadSize { get; private set; }
        public long Speed { get; private set; }
        public string TotalSizeString => GetByteString(TotalSize);
        public string DownloadSizeString => GetByteString(DownloadSize);
        public string SpeedString => $"{GetByteString(Speed)}/s";
        public float Progress { get; private set; }
        public DateTime CreatDate { get; init; }
        public DateTime StartDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public void Start(Action<DownloadTask> callBack)
        {
            var temp = "./temp/";
            if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
            Task.Factory.StartNew(() =>
            {
                long speed = 0;
                long lastspeed = 0;
                using var timer = new System.Timers.Timer(500)
                {
                    AutoReset = true,
                };
                timer.Elapsed += (o, e) =>
                {
                    Speed = speed + lastspeed;
                    lastspeed = speed;
                    speed = 0;
                };
                using var dq = new DownloadQueue.DownloadQueue();
                var task = dq.MakeDefault(Uri.ToString());
                task.StartCallback = () => StartDate = DateTime.Now;
                task.SizeCallback = (size) => TotalSize = size;
                task.DownloadCallback = (size) =>
                {
                    speed += size;
                    DownloadSize += size;
                    Progress = (float)DownloadSize / (float)TotalSize;
                    callBack(this);
                };
                task.Filename = temp + FileName;
                timer.Start();
                dq.DownloadFile(task);
                File.Move(temp + FileName, FileInfo.FullName);
                IsCompleted = true;
                CompletedDate = DateTime.Now;
                callBack(this);
            });
        }
        public async Task StartAsync(Action<DownloadTask> callBack)
        {
            var temp = "./temp/";
            if (!Directory.Exists(temp)) Directory.CreateDirectory(temp);
            long speed = 0;
            long lastspeed = 0;
            using var timer = new System.Timers.Timer(500)
            {
                AutoReset = true,
            };
            timer.Elapsed += (o, e) =>
            {
                Speed = speed + lastspeed;
                lastspeed = speed;
                speed = 0;
            };
            using var dq = new DownloadQueue.DownloadQueue();
            var task = dq.MakeDefault(Uri.ToString());

            task.StartCallback = () => StartDate = DateTime.Now;
            task.SizeCallback = (size) => TotalSize = size;
            task.DownloadCallback = (size) =>
            {
                speed += size;
                DownloadSize += size;
                Progress = (float)DownloadSize / (float)TotalSize;
                if(DownloadSize == TotalSize)
                {

                }
                callBack(this);
            };
            task.CompleteCallback = () =>
            {
                File.Move(temp + FileName, FileInfo.FullName);
                IsCompleted = true;
                CompletedDate = DateTime.Now;
                callBack(this);
            };
            task.CompleteCallbackBytes = b =>
            {
                var a = b;
            };
            task.Filename = temp + FileName;
            timer.Start();
            await dq.DownloadFileAsync(task);
            File.Move(temp + FileName, FileInfo.FullName);
            IsCompleted = true;
            CompletedDate = DateTime.Now;
            callBack(this);
        }
        private static string GetByteString(float value)
        {
            return (float)(value) switch
            {
                < 1024 => $"{value:0.00}b",
                < 1024 * 1024 => $"{(value /= 1024):0.00}Kb",
                < 1024 * 1024 * 1024 => $"{(value /= 1024 * 1024):0.00}Mb",
                _ => $"{value:0.00}Gb",
            };
        }
    }
}
