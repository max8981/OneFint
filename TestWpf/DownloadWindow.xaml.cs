using Downloader;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Windows.Shapes;

namespace TestWpf
{
    /// <summary>
    /// DownloadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadWindow : Window
    {
        DownloadController _download = new DownloadController();
        private readonly ConcurrentDictionary<string, ProgressBar> ps = new();
        public DownloadWindow()
        {
            InitializeComponent();
            _download.ProgressChanged += x =>
            {
                var package = (DownloadPackage)x;
                foreach (var chunk in package.Chunks)
                {
                    ps.AddOrUpdate(chunk.Id, key => new ProgressBar()
                    {
                        Name=key.ToString(),
                        Maximum = chunk.Length,
                        Value=chunk.Position,
                    },
                    (key, value) =>
                    {
                        value.Maximum = chunk.Length;
                        value.Value = chunk.Position;
                        return value;
                    });
                }
            };
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ps.Clear();
            panel.Children.Clear();
            var url = urlTextBox.Text;
            var package = new Downloader.DownloadPackage()
            {
                Address = url,
                FileName = "d:\\download\\wpf.mp4",
            };
            await _download.Download(package);
        }
    }
}
