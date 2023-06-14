using CefSharp;
using CefSharp.DevTools.WebAuthn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace 屏幕管理
{
    /// <summary>
    /// ExhibitionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionControl :System.Windows.Controls.UserControl, Interfaces.IExhibition
    {
        private System.Timers.Timer? _time;
        private readonly MediaElement _media = new()
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            LoadedBehavior = MediaState.Manual,
            Stretch = Stretch.Fill,
        };
        public ExhibitionControl(int id,string name, System.Drawing.Rectangle rectangle)
        {
            InitializeComponent();
            Id = id;
            Name = name;
            Width = rectangle.Width;
            Height = rectangle.Height;
            Margin = new Thickness(rectangle.X, rectangle.Y, 0, 0);
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Background = new SolidColorBrush(Colors.Transparent);
            grid.Background = new SolidColorBrush(Colors.Transparent);
        }
        public int Id { get; init; }
        public bool ShowAudio(string source, int playtime = 15)
        {
            var name = typeof(MediaElement).Name;
            Dispatcher.Invoke(() =>
            {
                _media.Source = new Uri(source);
                _media.Volume = 1;
                _media.Stop();
                _media.Play();
                Global.Pause += x => {
                    if (x)
                        Dispatcher.Invoke(() => _media.Pause());
                    else
                        Dispatcher.Invoke(() => _media.Play());
                };
                _media.Visibility= Visibility.Visible;
            });
            var result = _media.DelayHidden(name,playtime);
            Dispatcher.Invoke(() => _media.Visibility = Visibility.Visible);
            return result;
        }
        public void SetClock(Enums.ClockTypeEnum clockType, Models.BaseText text)
        {
            var name = "Clock";
            var clock = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not TextBlock clock)
                {
                    clock = new()
                    {
                        FontSize = text.FontSize,
                        Foreground = text.FontColor.ToBrush(),
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        //Background = text.BackgroundColor.ToBrush()
                    };
                    grid.Background = text.BackgroundColor.ToBrush();
                    grid.Children.Add(clock);
                    grid.RegisterName(name, clock);
                }
                return clock;
            });
            var _clockFormat = clockType switch
            {
                Enums.ClockTypeEnum.TYPE_1 => "yyyy-MM-dd ddd HH:mm:ss",
                Enums.ClockTypeEnum.TYPE_2 => "HH:mm:ss\nyyyy-MM-dd ddd",
                _ => "",
            };
            _time = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            _time.Elapsed += (o, e) => {
                var time = e.SignalTime.ToString(_clockFormat);
                Dispatcher.Invoke(() => clock.Text = time);
            };
            _time.Start();
        }
        public void SetText(Models.BaseText text)
        {
            var name = typeof(TextBlock).Name;
            var textBlock = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not TextBlock textBlock)
                {
                    textBlock = new()
                    {
                        TextWrapping = TextWrapping.WrapWithOverflow,
                    };
                    grid.Children.Add(textBlock);
                    grid.RegisterName(name, textBlock);
                }
                textBlock.Text = text.Text;
                textBlock.FontSize = text.FontSize;
                textBlock.Foreground = text.FontColor.ToBrush();
                textBlock.TextAlignment = ((int)text.Vertical).ToTextAlignment();
                textBlock.HorizontalAlignment = ((int)text.Horizontal).ToHorizontal();
                textBlock.VerticalAlignment = ((int)text.Vertical).ToVertical();
                textBlock.Visibility = Visibility.Visible;
                grid.Background = text.BackgroundColor.ToBrush();
                return textBlock;
            });
        }
        public void SetWeb(string? url)
        {
            var name = typeof(CefSharp.Wpf.ChromiumWebBrowser).Name;
            var web = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not CefSharp.Wpf.ChromiumWebBrowser web)
                {
                    web = new();
                    grid.Children.Add(web);
                    grid.RegisterName(name, web);
                }
                web.Address = url;
                web.Visibility = Visibility.Visible;
                return web;
            });
        }
        public bool ShowDownload(string title, Controllers.DownloadController.DownloadTask download, int playtime = 15)
        {
            var name = typeof(DownloadControl).Name;
            var downloader = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not DownloadControl downloader)
                {
                    downloader = new DownloadControl();
                    grid.Children.Add(downloader);
                    grid.RegisterName(name, downloader);
                }
                return downloader;
            });
            var result = downloader.DelayHidden(name, playtime, () =>
            {
                var speed = Controllers.DownloadController.GetByteString(download.DownloadSpeed);
                downloader.SetProgress(title, $"{speed}/s", download.Progress);
                return download.IsComplete;
            });
            Dispatcher.Invoke(() => downloader.Visibility = Visibility.Hidden);
            return !result;
        }
        public bool ShowImage(string source, int playtime = 15)
        {
            var name = typeof(System.Windows.Controls.Image).Name;
            var image = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not System.Windows.Controls.Image image)
                {
                    image = new()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch
                    };
                    grid.Children.Add(image);
                    grid.RegisterName(name, image);
                }
                image.Source = Global.GetImage(source);
                image.Visibility = Visibility.Visible;
                return image;
            });
            var result = image.DelayHidden(name, playtime);
            Dispatcher.Invoke(() => image.Visibility = Visibility.Hidden);
            return result;
        }
        public bool ShowText(Models.BaseText text, int playtime = 15) {
            var name = typeof(TextBlock).Name;
            var textBlock = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not TextBlock textBlock)
                {
                    textBlock = new()
                    {
                        TextWrapping = TextWrapping.WrapWithOverflow,
                    };
                    grid.Children.Add(textBlock);
                    grid.RegisterName(name, textBlock);
                }
                textBlock.Text = text.Text;
                textBlock.FontSize = text.FontSize;
                textBlock.Foreground = text.FontColor.ToBrush();
                textBlock.TextAlignment = ((int)text.Vertical).ToTextAlignment();
                textBlock.HorizontalAlignment = ((int)text.Horizontal).ToHorizontal();
                textBlock.VerticalAlignment = ((int)text.Vertical).ToVertical();
                textBlock.Visibility = Visibility.Visible;
                grid.Background = text.BackgroundColor.ToBrush();
                return textBlock;
            });
            var result = textBlock.DelayHidden(name, playtime);
            Dispatcher.Invoke(() => {
                textBlock.Visibility = Visibility.Hidden;
                grid.Background = "".ToBrush();
            });
            return result;
        }
        public bool ShowVideo(string source, bool mute, int playtime = 15)
        {
            var name = typeof(MediaElement).Name;
            var mediaElement = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not MediaElement mediaElement)
                {
                    mediaElement = new()
                    {
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        LoadedBehavior = MediaState.Manual,
                        Stretch = Stretch.Uniform,
                    };
                    mediaElement.MediaEnded += (o, e) => mediaElement.Visibility = Visibility.Hidden;
                    mediaElement.IsVisibleChanged += (o, e) =>
                    {
                        if ((bool)e.NewValue == false)
                        {
                            mediaElement.Pause();
                        }else
                            mediaElement.Play();
                    };
                    Global.Pause += x => {
                        if (x)
                            Dispatcher.Invoke(() => mediaElement.Pause());
                        else
                            Dispatcher.Invoke(() => mediaElement.Play());
                    };
                    grid.Children.Add(mediaElement);
                    grid.RegisterName(name, mediaElement);
                }
                mediaElement.Source = new Uri(source);
                mediaElement.Volume = mute ? 0 : 1;
                mediaElement.Stop();
                mediaElement.Play();
                mediaElement.Visibility = Visibility.Visible;
                return mediaElement;
            });
            var result = mediaElement.DelayHidden(name, playtime);
            Dispatcher.Invoke(() => mediaElement.Visibility = Visibility.Hidden);
            return result;
        }
        public bool ShowWeb(string? url, int playtime = 15)
        {
            var name = typeof(CefSharp.Wpf.ChromiumWebBrowser).Name;
            var web = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not CefSharp.Wpf.ChromiumWebBrowser web)
                {
                    web = new();
                    web.IsVisibleChanged += (o, e) =>
                    {
                        var visible = e.NewValue as bool?;
                        if (visible == false)
                            web.ToggleAudioMute();
                        else
                            web.ToggleAudioMute();
                    };
                    grid.Children.Add(web);
                    grid.RegisterName(name, web);
                }
                web.Address= url;
                web.Visibility = Visibility.Visible;
                web.Focus();
                return web;
            });
            var result = web.DelayHidden(name,playtime);
            Dispatcher.Invoke(()=>web.Visibility=Visibility.Hidden);
            return result;
        }
        ~ExhibitionControl()
        {
            _time?.Close();
        }
    }
}
