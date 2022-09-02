using Masuit.Tools;
using Masuit.Tools.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SharpCompress.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// DownloadProgressControl.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadProgressControl : UserControl
    {
        private readonly ConcurrentQueue<DownloadTask> _tasks = new();
        private bool _isDownloading;
        public DownloadProgressControl()
        {
            InitializeComponent();
            downloadProgressBar.Maximum = 100;
            downloadProgressBar.Minimum = 0;
            Visibility = Visibility.Hidden;
        }
        public void AddDownloadUri(DownloadTask task, Action<DownloadTask> callback)
        {
            _tasks.Enqueue(task);
            if (!_isDownloading)
            {
                Download(callback);
            }
        }
        private void Download(Action<DownloadTask> callback)
        {
            if (_tasks.TryDequeue(out var task))
            {
                _isDownloading = true;
                Dispatcher.Invoke(new Action(delegate
                {
                    Visibility = Visibility.Visible;
                }));
                task.Start(c =>
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        fileNameLabel.Content = task.FileInfo.Name;
                        downloadInfoLabel.Content = task.Speed;
                        downloadProgressBar.Value = task.TotalProgress;
                    }));
                    if (_tasks.IsEmpty)
                    {
                        _isDownloading = task.IsCompleted;
                        if (_isDownloading)
                        {
                            callback(task);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                Visibility = Visibility.Hidden;
                            }));
                        }
                    }
                    else
                    {
                        Download(callback);
                    }
                });
            }
        }
        public record DownloadTask
        {
            public DownloadTask(int id, string url,string? name = null, string? path = null)
            {
                Id = id;
                Uri = new Uri(url);
                FileName = name?.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0 ? name : System.Web.HttpUtility.UrlDecode(Uri.Segments.Last());
                Ext = System.IO.Path.GetExtension(FileName);
                FilePath = path?.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0 ? path : "./download"; ;
                CreatDate = DateTime.Now;
                Speed = "";
                if (!Directory.Exists(FilePath)) Directory.CreateDirectory(FilePath);
            }
            public int Id { get; init; }
            public string FileName { get; init; }
            public string Ext { get; init; }
            public string FilePath { get; init; }
            public FileInfo FileInfo => new($"{FilePath}/{FileName}");
            public Uri Uri { get; init; }
            public long FileLength { get; private set; }
            public string Speed { get; private set; }
            public float TotalProgress { get; private set; }
            public DateTime CreatDate { get; init; }
            public DateTime StartDate { get; private set; }
            public DateTime? CompletedDate { get; private set; }
            public bool IsCompleted { get; private set; }
            public async Task<bool> Start(System.Net.Http.HttpClient httpClient,TimeSpan timeout)
            {
                StartDate = DateTime.Now;
                using var response = await httpClient.GetAsync(Uri);
                if (response.IsSuccessStatusCode)
                {
                    using var stream = await response.Content.ReadAsStreamAsync();
                    FileLength = stream.Length;
                    using FileStream fs = File.Open(FileInfo.FullName, FileMode.Create);
                    var download = stream.CopyToAsync(fs);
                SpinWait.SpinUntil(() => {
                    TotalProgress = fs.Length / FileLength;
                    Console.WriteLine(TotalProgress);
                    return download.IsCompleted;
                }, timeout);
                    CompletedDate = DateTime.Now;
                    return download.IsCompleted;
                }
                else
                {
                    return false;
                }
            }
            public void Start(Action<DownloadTask> callback)
            {
                var mtd = new MultiThreadDownloader(Uri.ToString(),Environment.GetEnvironmentVariable("temp"),FileInfo.FullName, 8);
                mtd.Configure(req =>
                {
                    req.Referer = Uri.Host;
                    req.Headers.Add("Origin", Uri.Host);
                });
                mtd.TotalProgressChanged += (sender, e) =>
                {
                    var downloader = sender as MultiThreadDownloader;
                    TotalProgress = downloader == null ? 0 : downloader.TotalProgress;
                    Speed= GetByteString(downloader == null ? 0 : downloader.TotalSpeedInBytes);
                    callback(this);
                };
                //mtd.FileMergeProgressChanged += (sender, e) =>
                //{
                //    Console.WriteLine("下载完成");
                //};
                mtd.FileMergedComplete += (sender, e) => {
                    IsCompleted = true;
                    CompletedDate = DateTime.Now;
                    callback(this);
                };
                StartDate = DateTime.Now;
                mtd.Start();
            }
            private static string GetByteString(float value)
            {
                return (float)(value) switch
                {
                    < 1024 => $"{value:0.00}b/S",
                    < 1024 * 1024 => $"{(value /= 1024):0.00}Kb/s",
                    < 1024 * 1024 * 1024 => $"{(value /= 1024 * 1024):0.00}Mb/s",
                    _ => $"{value:0.00}Gb/S",
                };
            }
        }
    }
}
