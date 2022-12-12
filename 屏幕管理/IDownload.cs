using System;
using System.Collections.Generic;
using System.Text;

namespace 屏幕管理
{
    public interface IDownload
    {
        int Id { get; init; }
        string Name { get; init; }
        string Url { get; init; }
        string FileName { get; init; }
        long FileSize { get; set; }
        long DownloadSize { get; set; }
        float Progress { get; set; }
        long DownloadSpeed { get; set; }
        bool IsComplete { get; set; }
        Action CompleteCallback { get; }
        Action StartCallback { get; }
        Action<long> SizeCallback { get; }
        Action<long> DownloadCallback { get; }
        bool Start();
        IDownload CreateNew(int id,string url,string name);
    }
}
