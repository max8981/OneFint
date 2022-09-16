using DownloadQueue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ClientLibrary
{
    public class Downloader
    {
        private const long KB = 1024;
        private const long MB = 1024 ^ 2;
        private const long GB = 1024 ^ 3;
        private const long TB = 1024 ^ 4;
        private static string downloadPath = "./";
        private static readonly DownloadQueue.DownloadQueue downloadQueue = new();
        private static readonly ConcurrentDictionary<int, DownloadTask> _taskQueue = new();
        public static DownloadTask GetOrAddTask(int id,string url,string name)
        {
            if(!_taskQueue.TryGetValue(id,out var task))
            {
                task = new(id, url, name);
                if (!task.IsComplete)
                    task.Start();
            }
            return task;
        }
        public static void SetDownloadPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            downloadPath = path;
        }
        public static string GetByteString(long value)
        {
            return value switch
            {
                < KB => $"{value /= KB:0.00}KB",
                < MB => $"{value /= MB:0.00}MB",
                < GB => $"{value /= GB:0.00}GB",
                < TB => $"{value /= TB:0.00}TB",
                _ => $"{value}B",
            };
        }
        ~Downloader()
        {
            downloadQueue.Dispose();
        }
        public record DownloadTask
        {
            internal DownloadTask(int id,string url,string name)
            {
                Id = id;
                Url = url;
                Name = name;
                long ds = 0;
                Queue<long> longs = new();
                System.Timers.Timer timer = new(100)
                {
                    AutoReset = true,
                };
                timer.Elapsed += (o, e) =>
                {
                    longs.Enqueue(ds);
                    ds = 0;
                    if (longs.Count > 9)
                    {
                        DownloadSpeed = longs.Sum();
                        longs.Dequeue();
                    }
                };
                FileSize = 1;
                NetTask = downloadQueue.MakeDefault(url);
                NetTask.Filename = FileName = new FileInfo(downloadPath + Name).FullName;
                NetTask.StartCallback = () => timer.Start();
                NetTask.SizeCallback = o => FileSize = o;
                NetTask.DownloadCallback = o => { DownloadSize += o; ds += o; };
                NetTask.CompleteCallback = () => { IsComplete = true; timer.Close(); };
                IsComplete = File.Exists(FileName);
            }
            public int Id { get; init; }
            public string Name { get; init; }
            public string Url { get; init; }
            public string FileName { get; init; }
            public long FileSize { get; set; }
            public long DownloadSize { get; set; }
            public long DownloadSpeed { get; set; }
            public bool IsComplete { get; set; }
            public bool Start()
            {
                if (_taskQueue.TryAdd(Id, this))
                {
                    downloadQueue.AppendTask(NetTask);
                    return true;
                }
                return false;
            }
            internal NetTask NetTask { get; init; }
        }
    }
}
