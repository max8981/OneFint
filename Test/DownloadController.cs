using DownloadQueue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Test
{
    public class DownloadController
    {
        private const long KB = 1024;
        private const long MB = 1024 * KB;
        private const long GB = 1024 * MB;
        private const long TB = 1024 * GB;
        private static string materialPath = "./materials";
        private static readonly ConcurrentDictionary<int, DownloadTask> _taskQueue = new();
        private static readonly DownloadQueue.DownloadQueue downloadQueue = new();
        public static DownloadTask GetOrAddTask(int id, string url, int mId, int? deviceId, int? groupId)
        {
            var filename = new Uri(url).Segments.Last();
            var ext = Path.GetExtension(filename);
            var name = $"{id}{ext}";
            var task = _taskQueue.GetOrAdd(id, new DownloadTask(id, url, name));
            if (!task.IsComplete)
                task.Start();
            return task;
        }
        public static void SetMaterialsPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = "./materials";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            materialPath = path;
        }
        public static string GetByteString(long value)
        {
            return value switch
            {
                > TB => $"{value /= TB:0.00}TB",
                > GB => $"{value /= GB:0.00}GB",
                > MB => $"{value /= MB:0.00}MB",
                > KB => $"{value /= KB:0.00}KB",
                _ => $"{value}B",
            };
        }
        public static string[] MaterialFiles => Directory.GetFiles(materialPath);
        public class DownloadTask
        {
            private readonly NetTask _netTask;
            public DownloadTask(int id,string url,string name)
            {
                Id = id;
                Url = url;
                Name = name;
                CompleteCallback = () => { };
                StartCallback= () => { };
                SizeCallback = o => { };
                DownloadCallback = o => { };
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
                _netTask = downloadQueue.MakeDefault(url);
                var tempFile = Path.GetTempFileName();
                FileName = new FileInfo(Path.Combine(materialPath, Name)).FullName;
                _netTask.Filename = tempFile;
                _netTask.StartCallback = () => timer.Start();
                _netTask.SizeCallback = o => FileSize = o;
                _netTask.DownloadCallback = o => { DownloadSize += o; ds += o; Progress = (float)DownloadSize / FileSize; };
                _netTask.CompleteCallback = () =>
                {
                    timer.Close();
                    _ = Task.Factory.StartNew(() =>
                    {
                        SpinWait.SpinUntil(() =>
                        {
                            try
                            {
                                File.Move(tempFile, FileName, true);
                                return true;
                            }
                            catch
                            {
                                return false;
                            }
                        });
                        IsComplete = true;
                        CompleteCallback();
                    });
                };
                IsComplete = File.Exists(FileName);
            }
            internal int Id { get; init; }
            internal string Name { get; init; }
            internal string Url { get; init; }
            internal string FileName { get; init; }
            internal long FileSize { get; set; }
            internal long DownloadSize { get; set; }
            internal float Progress { get; set; }
            internal long DownloadSpeed { get; set; }
            internal bool IsComplete { get; set; }
            internal Action CompleteCallback { get; }
            internal Action StartCallback { get; }
            internal Action<long> SizeCallback { get; }
            internal Action<long> DownloadCallback { get; }
            internal bool Start()
            {
                if (_taskQueue.TryAdd(Id, this))
                {
                    downloadQueue.AppendTask(_netTask);
                    return true;
                }
                return false;
            }
        }
    }
}
