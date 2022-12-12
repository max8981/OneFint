using System;
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
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Client.CustomControls
{
    /// <summary>
    /// ExhibitionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionControl : UserControl,ClientCore.IExhibition
    {
        private readonly MediaElement _media = new()
        {
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            LoadedBehavior = MediaState.Manual,
            Stretch = Stretch.Fill,
        };
        private readonly System.Windows.Controls.Image _image = new()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };
        private readonly TextBlock _text = new()
        {
            //Margin = new Thickness(0, 0, 0, 0),
            TextWrapping = TextWrapping.WrapWithOverflow,
        };
        private readonly CefSharp.Wpf.ChromiumWebBrowser _browser = new();
        private readonly DownloadControl _download = new();
        public ExhibitionControl()
        {
            InitializeComponent();
        }
        public int Id { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public bool ShowAudio(string source, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                _media.Source = new Uri(source);
                _media.Volume = 1;
                _media.Stop();
                _media.Play();
                return _media.DelayHidden(playtime);
            });
        }

        public void ShowClock(int clockType, string? color, int size = 0, string? bgcolor = null)
        {
            var _clockFormat = clockType switch
            {
                0 => "yyyy-MM-dd ddd HH:mm:ss",
                1 => "HH:mm:ss\nyyyy-MM-dd ddd",
                _ => "",
            };
            System.Timers.Timer timer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            timer.Elapsed += (o, e) => {
                var text = e.SignalTime.ToString(_clockFormat);
                ShowText(text, color, size, bgcolor, 2, 2);
            };
            timer.Start();
        }

        public bool ShowDownload(string title, ClientCore.IDownload download, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                return _download.DelayHidden(playtime, () =>
                {
                    var speed = ClientCore.DownloadHelper.GetByteString(download.DownloadSpeed);
                    _download.SetProgress(title, $"{speed}/s", download.Progress);
                    return download.IsComplete;
                });
            });
        }

        public bool ShowImage(string source, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                _image.Source = ControlEx.GetImage(source);
                return _image.DelayHidden(playtime);
            });
        }

        public bool ShowText(string? text, string? color, int size = 0, string? bgcolor = null, int horizontal = 0, int vertical = 0, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                _text.Text = text;
                _text.FontSize = size > 0 ? size : 14d;
                _text.Foreground = color.ToBrush();
                _text.TextAlignment = vertical.ToTextAlignment();
                _text.HorizontalAlignment = horizontal.ToHorizontal();
                _text.VerticalAlignment = vertical.ToVertical();
                _text.Background = bgcolor.ToBrush();
                //Background= bgcolor.ToBrush();
                return _text.DelayHidden(playtime);
            });
        }

        public bool ShowVideo(string source, bool mute, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                _media.Source = new Uri(source);
                _media.Volume = mute ? 0 : 1;
                _media.Stop();
                _media.Play();
                return _media.DelayHidden(playtime);
            });
        }

        public bool ShowWeb(string? url, int playtime = 15)
        {
            return Dispatcher.Invoke(() =>
            {
                _browser.Address = url;
                return _browser.DelayHidden(playtime);
            });
        }
    }
}
