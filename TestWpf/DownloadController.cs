using Downloader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWpf
{
    internal class DownloadController
    {
        private readonly DownloadConfiguration _configuration;
        private readonly DownloadService _service;
        private DateTime _lastTime;

        public DownloadController()
        {
            _configuration = new DownloadConfiguration()
            {
                BufferBlockSize = 1024,
                ChunkCount = 5,
                MaximumBytesPerSecond = 0,
                MaxTryAgainOnFailover = int.MaxValue,
                ParallelDownload = true,
                ParallelCount = 5,
                Timeout = 5000,
            };
            _service = new DownloadService(_configuration);
            _service.DownloadStarted += (o, e) =>
            {
                var pack = _service.Package;
                var json = System.Text.Json.JsonSerializer.Serialize(pack);
                System.IO.File.WriteAllText("d:\\download\\pack", json);
            };
            _service.DownloadProgressChanged += (o, e) =>
            {
                if ((DateTime.Now - _lastTime) > TimeSpan.FromMilliseconds(100))
                {
                    Console.Clear();
                    var s = o as DownloadService;
                    var package = s.Package;
                    Console.CursorTop = 0;
                    Console.CursorLeft = 0;
                    Console.WriteLine($"{package.FileName} {e.ProgressId} {e.ActiveChunks} {e.ProgressPercentage} {e.TotalBytesToReceive}");
                    foreach (var c in package.Chunks)
                    {
                        Console.WriteLine($"{c.Id} {c.Start} {c.End} {c.Position} {c.Length}");
                    }
                    _lastTime = DateTime.Now;
                    ProgressChanged?.Invoke(package);
                }
            };
            _service.ChunkDownloadProgressChanged += (o, e) =>
            {
                //var s = o as DownloadService;
                //var t = s.Package;
                //Console.CursorTop = e.ActiveChunks+10;
                //Console.CursorLeft = 0;
                //Console.WriteLine($"{t.FileName} {e.ProgressId} {e.ActiveChunks} {e.ProgressPercentage} {e.TotalBytesToReceive}");
            };
            _service.DownloadFileCompleted += (o, e) =>
            {
                var s = o as DownloadService;
                var p = e.UserState as DownloadPackage;
                Console.WriteLine($"{p.FileName} {p.IsSaveComplete} {e.Cancelled} {e.Error}");
            };
            if (System.IO.File.Exists("d:\\download\\pack"))
            {
                var json = System.IO.File.ReadAllText("d:\\download\\pack");
                var pack = System.Text.Json.JsonSerializer.Deserialize<DownloadPackage>(json);
                _service.DownloadFileTaskAsync(pack);
            }
        }
        public bool IsCompleted => _service.Package.IsSaveComplete;
        public async Task Download(string url, string file)
        {
            await _service.DownloadFileTaskAsync(url, file);
        }
        public async Task Download(string url)
        {
            await _service.DownloadFileTaskAsync(url, new DirectoryInfo("d:\\download\\"));
        }
        public async Task Download(DownloadPackage package, string file = "")
        {
            using var s = await _service.DownloadFileTaskAsync(package);
            if (!string.IsNullOrWhiteSpace(file))
            {
                using var fs = System.IO.File.Create(file);
                await s.CopyToAsync(fs);
            }
        }
        public DownloadPackage Package => _service.Package;
        public Action<DownloadPackage>? ProgressChanged;
        public void Cancel()
        {
            _service.CancelAsync();
            var pack = _service.Package;
            var json = System.Text.Json.JsonSerializer.Serialize(pack);
            System.IO.File.WriteAllText("d:\\download\\pack", json);
            Console.WriteLine(json);
        }
        ~DownloadController()
        {
            Cancel();
        }
    }
}
