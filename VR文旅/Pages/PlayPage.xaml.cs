using CefSharp;
using System;
using System.Collections.Generic;
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

namespace VR文旅.Pages
{
    /// <summary>
    /// PlayPage.xaml 的交互逻辑
    /// </summary>
    public partial class PlayPage : Page
    {
        public event Action<bool>? Show;
        private readonly System.Timers.Timer _timer;
        private Point _lastPoint = new();
        private Point _mouseMovePoint = new();
        private bool _stopPlay;
        public PlayPage()
        {
            InitializeComponent();
            _timer = new()
            {
                Interval = 1000,
                AutoReset = true,
            };
            int timespan = 0;
            _timer.Elapsed += async (o, e) =>
            {
                var point = Global.GetMousePoint();
                if (point.Equals(_lastPoint))
                    timespan++;
                else
                    timespan = 0;
                _lastPoint = point;
                if (timespan > 5)
                {
                    timespan = 0;
                    _timer.Stop();
                    var standby = await Models.Standbys.GetStandbiesAsync();
                    AutoPlay(standby);
                }
            };
            _timer.Start();
        }
        public bool ShowWeb(string? url)
        {
            HiddenAll();
            var name = typeof(Controls.WebBrowserControl).Name;
            var result = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not Controls.WebBrowserControl webBrowser)
                {
                    webBrowser = new()
                    {
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                    };
                    webBrowser.Return += () => Show?.Invoke(false);
                    grid.Children.Add(webBrowser);
                    grid.RegisterName(name, webBrowser);
                }
                webBrowser.Navigation(url);
                webBrowser.Visibility = Visibility.Visible;
                return webBrowser.IsLoaded;
            });
            Show?.Invoke(true);
            return result;
        }
        public void Close()
        {
            _stopPlay = true;
            _timer.Stop();
            _timer.Close();
        }
        private bool ShowImage(string? source, int playtime)
        {
            HiddenAll();
            if (string.IsNullOrEmpty(source)) return true;
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
                    image.MouseMove += WatchMouseMove;
                    grid.Children.Add(image);
                    grid.RegisterName(name, image);
                }
                image.Source = new BitmapImage(new Uri(source));
                image.Visibility = Visibility.Visible;
                return image;
            });
            var result = !SpinWait.SpinUntil(() => Global.IsMouseMove(), playtime * 1000);
            Dispatcher.Invoke(() => image.Visibility = Visibility.Hidden);
            return result;
        }
        private bool ShowVideo(string? source, bool mute, int playtime)
        {
            HiddenAll();
            if (string.IsNullOrEmpty(source)) return true;
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
                        Stretch = Stretch.Fill,
                    };
                    mediaElement.MouseMove += WatchMouseMove;
                    mediaElement.MediaEnded += (o, e) => mediaElement.Visibility = Visibility.Hidden;
                    mediaElement.IsVisibleChanged += (o, e) =>
                    {
                        if (e.NewValue is bool value)
                            if (!value)
                                mediaElement.Stop();
                            else
                                mediaElement.Play();
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
            var result = !SpinWait.SpinUntil(() => Global.IsMouseMove(), playtime * 1000);
            Dispatcher.Invoke(() => mediaElement.Visibility = Visibility.Hidden);
            return result;
        }
        private void AutoPlay(IEnumerable<Models.Standbys.Standby> standbies)
        {
            Task.Factory.StartNew(() =>
            {
                if (standbies.Any())
                {
                    _stopPlay = false;
                    Show?.Invoke(true);
                    var enumerator = standbies.GetEnumerator();
                    while (enumerator.MoveNext()&&!_stopPlay)
                        if (enumerator.Current.Status == 1)
                            if (DateTime.Now > enumerator.Current.Start && DateTime.Now < enumerator.Current.End)
                                switch (enumerator.Current.ResourceType)
                                {
                                    case Models.Standbys.ResourceTypeEnum.Image:
                                        ShowImage(enumerator.Current.ResourceUrl, enumerator.Current.Duration);
                                        break;
                                    case Models.Standbys.ResourceTypeEnum.Video:
                                        ShowVideo(enumerator.Current.ResourceUrl, enumerator.Current.Mute, enumerator.Current.Duration);
                                        break;
                                    default: break;
                                }
                }
                Show?.Invoke(false);
                _timer.Start();
            });
        }
        private void WatchMouseMove(object sender, MouseEventArgs e)
        {
            var point = e.MouseDevice.GetPosition(this);
            if (_mouseMovePoint == new Point())
                _mouseMovePoint = point;
            else
                _stopPlay = !_mouseMovePoint.Equals(point);
        }
        private void HiddenAll()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (FrameworkElement item in grid.Children)
                    item.Visibility = Visibility.Hidden;
            });
        }
        ~PlayPage() {
            _timer.Stop();
            _timer.Close();
        }
    }
}
