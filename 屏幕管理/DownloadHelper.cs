using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 屏幕管理
{
    internal class DownloadHelper
    {
        private const long KB = 1024;
        private const long MB = 1024 * KB;
        private const long GB = 1024 * MB;
        private const long TB = 1024 * GB;
        private static string materialPath = "./materials";
        private static readonly ConcurrentDictionary<int, IDownload> _taskQueue = new();
        public static IDownload GetOrAddTask(int id, string url, int mId, int? deviceId, int? groupId)
        {
            if (!_taskQueue.TryGetValue(id, out var task))
            {
                var filename = new Uri(url).Segments.Last();
                var ext = Path.GetExtension(filename);
                var name = $"{id}{ext}";
                task = task.CreateNew(id, url, name);
                if (!task.IsComplete)
                    task.Start();
            }
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
    }
}
