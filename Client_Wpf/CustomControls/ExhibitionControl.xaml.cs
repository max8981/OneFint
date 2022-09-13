using ClientLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Client_Wpf.CustomControls.ControlHelper;
using static System.Net.Mime.MediaTypeNames;

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// ExhibitionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionControl : UserControl
    {
        const string clockType1 = "yyyy-MM-dd ddd HH:mm:ss";
        const string clockType2 = "HH:mm:ss\nyyyy-MM-dd ddd";
        private string _clockFormat = "";
        private System.Timers.Timer _timer = null!;
        private readonly ConcurrentQueue<MediaContent> medias = new();
        public ExhibitionControl(string name,int w,int h,int x,int y,int z)
        {
            InitializeComponent();
            medias = new();
            Name = name;
            Width = w;
            Height = h;
            Margin = new Thickness(x, y, 0, 0);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Panel.SetZIndex(this, z);
        }
        public void ShowText(int id,string content,Brush fore,Brush back,double size,int textAlignment, int horizontalAlignment,int verticalAlignment)
        {
            var name = $"text{id}";
            Dispatcher.Invoke(() =>
            {
                if(grid.FindName(name) is not TextBlock textBlock)
                {
                    textBlock = new TextBlock
                    {
                        Name = name,
                        Margin = new Thickness(0, 0, 0, 0),
                        Text = content,
                        FontSize = size,
                        Foreground = fore,
                        Background = back,
                        TextAlignment = GetTextAlignment(textAlignment),
                        HorizontalAlignment = GetHorizontal(horizontalAlignment),
                        VerticalAlignment = GetVertical(verticalAlignment)
                    };
                    grid.Background = back;
                    grid.Children.Add(textBlock);
                    grid.RegisterName(name, textBlock);
                }
            });
        }
        public void ShowWebBrowser(int id,string source)
        {
            var name = $"wb{id}";
            WebBrowser webBrowser=
            Dispatcher.Invoke(() =>
            {
                if(grid.FindName(name) is not WebBrowser webBrowser)
                {
                    webBrowser = new WebBrowser();
                    var script = "document.body.style.overflow='hidden'";
                    webBrowser.LoadCompleted += (o, e) =>
                    {
                        webBrowser.InvokeScript("execScript", new object[] { script, "JavaScript" });
                    };
                    webBrowser.Navigate(source);
                    grid.Children.Add(webBrowser);
                    FieldInfo? fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fiComWebBrowser != null)
                    {
                        object? objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                        if (objComWebBrowser != null)
                        {
                            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
                        }
                    }
                    grid.RegisterName(name, webBrowser);
                }
                return webBrowser;
            });
        }
        public void ShowClock(int type, Brush fore, Brush back, double size)
        {
            var clock = Dispatcher.Invoke(() =>
            {
                TextBlock clock = new()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Text = "clock",
                    FontSize = size,
                    Foreground = fore,
                    Background = back,
                    TextAlignment = GetTextAlignment(2),
                    HorizontalAlignment = GetHorizontal(2),
                    VerticalAlignment = GetVertical(2)
                };
                grid.Children.Add(clock);
                grid.Background = back;
                return clock;
            });
            _clockFormat = type switch
            {
                1 => clockType1,
                2 => clockType2,
                _ => "",
            };
            _timer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            _timer.Elapsed += (o, e) =>
                Dispatcher.Invoke(() =>
                clock.Text = e.SignalTime.ToString(_clockFormat));
            _timer.Start();
        }
        public void ShowExhibition()
        {
            medias.Clear();
            _timer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            _timer.Elapsed += AutoPlay;
            _timer.Start();
        }
        public void AddMedia(MediaContent content)
        {
            medias.Enqueue(content);
        }
        private void AutoPlay(object? source,ElapsedEventArgs elapsed)
        {
            if (medias.TryDequeue(out var content))
            {
                if (elapsed.SignalTime > content.StartAt)
                {
                    _timer.Interval = content.Duration * 1000;
                    switch (content.MediaType)
                    {
                        case MediaContent.MediaTypeEnum.Video:
                            SetVideo(content);
                            break;
                        case MediaContent.MediaTypeEnum.Image:
                            SetImage(content);
                            break;
                        case MediaContent.MediaTypeEnum.Web:
                            SetWeb(content.Source);
                            break;
                        case MediaContent.MediaTypeEnum.Text:
                            SetText(content);
                            break;
                    }
                }
                if (elapsed.SignalTime < content.EndAt)
                {
                    medias.Enqueue(content);
                }
            }
            else
            {
                _timer.Interval = 1000;
            }
        }
        private void SetVideo(MediaContent content)
        {
            Hidden();
            GetMateroal(content, o =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (grid.FindName("video") is not MediaElement mediaElement)
                    {
                        mediaElement = new()
                        {
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            LoadedBehavior = MediaState.Manual,
                        };
                        grid.Children.Add(mediaElement);
                        grid.RegisterName("video", mediaElement);
                    }
                    mediaElement.Visibility = Visibility.Visible;
                    mediaElement.Source = new Uri(o.Source);
                    mediaElement.Volume = o.Mute ? 0 : 0.5;
                    mediaElement.Play();
                });
            });
        }
        private void SetImage(MediaContent content)
        {
            Hidden();
            GetMateroal(content, o =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (grid.FindName("image") is not System.Windows.Controls.Image image)
                    {
                        image = new()
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        grid.Children.Add(image);
                        grid.RegisterName("image", image);
                    }
                    image.Visibility = Visibility.Visible;
                    image.Source = new BitmapImage(new Uri(content.Source));
                });
            });
        }
        private void SetWeb(string source)
        {
            Hidden();
            Dispatcher.Invoke(() =>
            {
                if(grid.FindName("web")is not WebBrowser webBrowser)
                {
                    webBrowser = new();
                    var script = "document.body.style.overflow='hidden'";
                    webBrowser.LoadCompleted += (o, e) =>
                    {
                        webBrowser.InvokeScript("execScript", new object[] { script, "JavaScript" });
                    };
                }
                webBrowser.Visibility = Visibility.Visible;
                webBrowser.Navigate(source);
                grid.Children.Add(webBrowser);
                grid.RegisterName("web", webBrowser);
                FieldInfo? fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fiComWebBrowser != null)
                {
                    object? objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                    if (objComWebBrowser != null)
                    {
                        objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
                    }
                }
            });
        }
        private void SetText(MediaContent content)
        {
            Hidden();
            Dispatcher.Invoke(() =>
            {
                if(grid.FindName("text") is not TextBlock textBlock)
                {
                    textBlock = new();
                    grid.Children.Add(textBlock);
                    grid.RegisterName("text", textBlock);
                }
                textBlock.Margin = new Thickness(0, 0, 0, 0);
                textBlock.Text = content.Source;
                textBlock.FontSize = content.FontSize;
                textBlock.Foreground = content.FontColor;
                textBlock.Background = content.BackRound;
                textBlock.TextAlignment = content.TextAlignment;
                textBlock.HorizontalAlignment = content.HorizontalAlignment;
                textBlock.VerticalAlignment = content.VerticalAlignment;
                grid.Background = content.BackRound;
            });
        }
        private void GetMateroal(MediaContent content,Action<MediaContent> callBack)
        {
            var name = $"download{content.Id}";
            var downloadTask = new DownloadTask(content.Id, content.Source, $"{content.Name}");
            if (!downloadTask.IsExists)
            {
                Dispatcher.Invoke(async () =>
                {
                    if (grid.FindName(name) is not DownloadControl downloader)
                    {
                        downloader = new DownloadControl();
                        grid.Children.Add(downloader);
                        grid.RegisterName(name, downloader);
                        await downloadTask.StartAsync(o =>
                        {
                            downloader.SetProgress(name, $"{o.SpeedString}[{o.DownloadSizeString}/{o.TotalSizeString}]", o.Progress);
                        });
                        grid.Children.Remove(downloader);
                        grid.UnregisterName(name);
                        content.Source = downloadTask.Source;
                        callBack(content);
                    }
                    downloader.Visibility = Visibility.Visible;
                });
            }
            else
            {
                content.Source = downloadTask.Source;
                callBack(content);
            }
        }
        private void Hidden()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (FrameworkElement item in grid.Children)
                {
                    item.Visibility = Visibility.Hidden;
                }
            });
        }
        ~ExhibitionControl()
        {
            _timer.Close();
        }
        public record MediaContent
        {
            public int Id { get; set; }
            public MediaTypeEnum MediaType { get; set; }
            public string Name { get; set; }
            public string Source { get; set; }
            public int Duration { get; set; }
            public bool Mute { get; set; }
            public DateTime StartAt { get; set; }
            public DateTime EndAt { get; set; }
            public string Text { get; set; }
            public Brush BackRound { get; set; }
            public Brush FontColor { get; set; }
            public double FontSize { get; set; }
            public System.Windows.TextAlignment TextAlignment { get; set; }
            public System.Windows.HorizontalAlignment HorizontalAlignment { get; set; }
            public System.Windows.VerticalAlignment VerticalAlignment { get; set; }

            public enum MediaTypeEnum
            {
                Video=2,
                Image=3,
                Web=4,
                Text=5,
            }
        }
    }
}
