using ClientLibrary;
using ClientLibrary.Enums;
using ClientLibrary.Models;
using Microsoft.VisualBasic;
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
using static ClientLibrary.Models.Content;
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
        //private System.Timers.Timer _timer = null!;
        //private readonly ConcurrentQueue<MediaContent> medias = new();
        private readonly ConcurrentDictionary<int, MediaContent> _medias = new();
        private readonly ConcurrentQueue<MediaContent> _defaultContents = new();
        private readonly ConcurrentQueue<MediaContent> _normalContents = new();
        private readonly ConcurrentQueue<MediaContent> _emergencyContents = new();
        private readonly System.Timers.Timer _timer = new() { AutoReset = true, Interval = 1000 };
        public ExhibitionControl(string name,int w,int h,int x,int y,int z)
        {
            InitializeComponent();
            Name = name;
            Width = w;
            Height = h;
            Margin = new Thickness(x, y, 0, 0);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Background = new SolidColorBrush(Colors.Transparent);
            grid.Background = new SolidColorBrush(Colors.Transparent);
            Panel.SetZIndex(this, z);
            _timer.Start();
        }
        public void ShowText(int id,string content,string fore,string back,double size,int textAlignment, int horizontalAlignment,int verticalAlignment)
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
                    };
                    grid.Children.Add(textBlock);
                    grid.RegisterName(name, textBlock);
                }
                textBlock.Text = content;
                textBlock.FontSize = size;
                textBlock.Foreground = GetBrush(fore);
                textBlock.Background = GetBrush(back);
                textBlock.TextAlignment = GetTextAlignment(textAlignment);
                textBlock.HorizontalAlignment = GetHorizontal(horizontalAlignment);
                textBlock.VerticalAlignment = GetVertical(verticalAlignment);
                grid.Background = GetBrush(back);
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
        public void ShowClock(int type, string fore, string back, double size)
        {
            var clock = Dispatcher.Invoke(() =>
            {
                TextBlock clock = new()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Text = "clock",
                    FontSize = size,
                    Foreground = GetBrush(fore),
                    Background = GetBrush(back),
                    TextAlignment = GetTextAlignment(2),
                    HorizontalAlignment = GetHorizontal(2),
                    VerticalAlignment = GetVertical(2)
                };
                grid.Children.Add(clock);
                grid.Background = GetBrush(back); 
                return clock;
            });
            _clockFormat = type switch
            {
                1 => clockType1,
                2 => clockType2,
                _ => "",
            };
            System.Timers.Timer timer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            _timer.Elapsed += (o, e) =>
                Dispatcher.Invoke(() =>
                clock.Text = e.SignalTime.ToString(_clockFormat));
            timer.Start();
        }
        public void ShowExhibition()
        {
            System.Timers.Timer timer = new()
            {
                AutoReset = true,
                Interval = 1000,
            };
            //_timer.Elapsed += AutoPlay;
            //timer.Start();
        }
        public void AddContent(ContentTypeEnum type,MediaContent content)
        {
            switch (type)
            {
                case ContentTypeEnum.NORMAL:
                    _normalContents.Enqueue(content);
                    break;
                case ContentTypeEnum.EMERGENCY:
                    _emergencyContents.Enqueue(content);
                    break;
                case ContentTypeEnum.NEWFLASH:
                    break;
                case ContentTypeEnum.DEFAULT:
                    _defaultContents.Enqueue(content);
                    break;
            }
        }
        public void Clear(ContentTypeEnum? type =null)
        {
            switch (type)
            {
                case ContentTypeEnum.NORMAL:
                    _normalContents.Clear();
                    break;
                case ContentTypeEnum.EMERGENCY:
                    _emergencyContents.Clear();
                    break;
                case ContentTypeEnum.NEWFLASH:
                    break;
                case ContentTypeEnum.DEFAULT:
                    _defaultContents.Clear();
                    break;
                default:
                    _normalContents.Clear();
                    _emergencyContents.Clear();
                    _defaultContents.Clear();
                    break;
            }
            _timer.Interval = 1000;
        }
        private void AutoPlay(object? source,ElapsedEventArgs elapsed)
        {
            var date = elapsed.SignalTime;
            Hidden();
            if (!GetMediaContent(_emergencyContents, date, out var content))
            {
                if (!GetMediaContent(_normalContents, date, out content))
                {
                    if (!GetMediaContent(_defaultContents, date, out content))
                    {
                        //Hidden();
                    }
                }
            }


 
            if (content != null)
            {
                SetContent(content);
                _timer.Interval = content.Duration * 1000;
            }
            else
            {
                _timer.Interval = 1000;
            }
        }
        private static bool GetMediaContent(ConcurrentQueue<MediaContent> contents, DateTime date,out MediaContent? content)
        {
            bool result = false;
            if (contents.TryDequeue(out content))
            {
                if(date < content.EndAt&& date > content.StartAt)
                {
                    contents.Enqueue(content);
                    result = true;
                }
                else
                {
                    return GetMediaContent(contents, date, out content);
                }
            }
            return result;
        }
        private void SetContent(MediaContent content)
        {
            switch (content.MediaType)
            {
                case MediaContent.MediaTypeEnum.Video:
                    SetVideo(content);
                    break;
                case MediaContent.MediaTypeEnum.Image:
                    SetImage(content);
                    break;
                case MediaContent.MediaTypeEnum.Web:
                    SetWeb(content);
                    break;
                case MediaContent.MediaTypeEnum.Text:
                    SetText(content);
                    break;
            }
            //SpinWait.SpinUntil(() => DateTime.Now > content.EndAt, content.Duration * 1000);
            //Hidden();
            //PlayNext();
        }
        private void SetVideo(MediaContent content)
        {
            GetMateroal(content, o =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    if (grid.FindName("video") is not MediaElement mediaElement)
                    {
                        mediaElement = new()
                        {
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            LoadedBehavior = MediaState.Manual,
                            Stretch=Stretch.Fill,
                        };
                        mediaElement.IsVisibleChanged += (o, e) =>
                        {
                            var visible = e.NewValue as bool?;
                            if (visible == false)
                            {
                                mediaElement.Pause();
                            }
                            else
                            {
                                mediaElement.Play();
                            }
                        };
                        grid.Children.Add(mediaElement);
                        grid.RegisterName("video", mediaElement);
                    }
                    mediaElement.Visibility = Visibility.Visible;
                    mediaElement.Source = new Uri(o.Source);
                    mediaElement.Volume = o.Mute ? 0 : 1;
                    mediaElement.Play();
                });
            });
        }
        private void SetImage(MediaContent content)
        {
            GetMateroal(content, o =>
            {
                var image =
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
                    image.Source = new BitmapImage(new Uri(content.Source));
                    image.Visibility = Visibility.Visible;
                    return image;
                });
                SpinWait.SpinUntil(() => DateTime.Now > content.EndAt, content.Duration * 1000);
                Dispatcher.Invoke(() => image.Visibility = Visibility.Hidden);
            });
            PlayNext();
        }
        private void SetWeb(MediaContent content)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if(grid.FindName("web")is not WebBrowser webBrowser)
                {
                    webBrowser = new();
                    grid.Children.Add(webBrowser);
                    grid.RegisterName("web", webBrowser);
                    var script = "document.body.style.overflow='hidden'";
                    webBrowser.LoadCompleted += (o, e) =>
                    {
                        webBrowser.InvokeScript("execScript", new object[] { script, "JavaScript" });
                    };
                    webBrowser.Navigated+=(o,e)=> webBrowser.Visibility = Visibility.Visible;
                }
                webBrowser.Navigate(content.Source);
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
            Dispatcher.BeginInvoke(() =>
            {
                if(grid.FindName("text") is not TextBlock textBlock)
                {
                    textBlock = new()
                    {
                        Margin = new Thickness(0, 0, 0, 0)
                    };
                    grid.Children.Add(textBlock);
                    grid.RegisterName("text", textBlock);
                }
                textBlock.Text = content.Source;
                textBlock.FontSize = content.FontSize;
                textBlock.Foreground = GetBrush(content.FontColor);
                textBlock.Background = GetBrush(content.BackRound);
                textBlock.TextAlignment = content.TextAlignment;
                textBlock.HorizontalAlignment = content.HorizontalAlignment;
                textBlock.VerticalAlignment = content.VerticalAlignment;
                grid.Background = GetBrush(content.BackRound);
                textBlock.Visibility = Visibility.Visible;
            });
        }
        public void PlayNext()
        {
            if(!GetMediaContent(_emergencyContents, DateTime.Now, out var content))
                if (!GetMediaContent(_normalContents, DateTime.Now, out content))
                    if (!GetMediaContent(_defaultContents, DateTime.Now, out content)) { }
            if (content != null)
            {
                SetContent(content);
            }
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
        private static Brush? GetBrush(string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(colorString);
                return new SolidColorBrush(color);
            }
        }
        public void Close()
        {
            _timer.Close();
            this.Close();
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
            public string BackRound { get; set; }
            public string FontColor { get; set; }
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
