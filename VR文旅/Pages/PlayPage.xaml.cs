using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
using static VR文旅.Models.Standbys;

namespace VR文旅.Pages
{
    /// <summary>
    /// PlayPage.xaml 的交互逻辑
    /// </summary>
    public partial class PlayPage : Page
    {
        public event Action<bool>? Show;
        private readonly System.Timers.Timer _timer;
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
                timespan = Global.IsMouseMove() ? 0 : timespan + 1;
                if (timespan > Systems.Config.WaitTime)
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
            var result = false;
            try
            {
                HiddenAll();
                var name = typeof(Controls.WebBrowserControl).Name;
                result = Dispatcher.Invoke(() =>
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
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "ShowWeb", url);
            }
            return result;
        }
        public void Close()
        {
            _stopPlay = true;
            _timer.Stop();
            _timer.Close();
        }
        private bool ShowImage(Models.Standbys.Standby standby)
        {
            var result = false;
            try
            {
                HiddenAll();
                if (string.IsNullOrEmpty(standby.ResourceUrl)) return true;
                if (Download(standby, out var file))
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
                        image.Source = Global.GetBitmap(file);
                        image.Visibility = Visibility.Visible;
                        return image;
                    });
                    result = !SpinWait.SpinUntil(() => Global.IsMouseMove(), standby.Duration * 1000);
                    Dispatcher.Invoke(() => image.Visibility = Visibility.Hidden);
                }
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "ShowImage", JsonSerializer.Serialize(standby));
            }
            return result;
        }
        private bool ShowVideo(Models.Standbys.Standby standby)
        {
            var result = false;
            try
            {
                HiddenAll();
                if (string.IsNullOrEmpty(standby.ResourceUrl)) return true;
                if (Download(standby, out var file))
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
                                Stretch = Stretch.Fill,
                            };
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
                        mediaElement.Source = new Uri(file.FullName);
                        mediaElement.Volume = standby.Mute ? 0 : 1;
                        mediaElement.Stop();
                        mediaElement.Play();
                        mediaElement.Visibility = Visibility.Visible;
                        return mediaElement;
                    });
                    result = !SpinWait.SpinUntil(() => Global.IsMouseMove(), standby.Duration * 1000);
                    Dispatcher.Invoke(() => mediaElement.Visibility = Visibility.Hidden);
                }
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "ShowVideo", JsonSerializer.Serialize(standby));
            }
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
                    foreach (var standby in standbies)
                        if (!_stopPlay)
                            if (standby.Status == 1)
                                if (DateTime.Now > standby.Start && DateTime.Now < standby.End)
                                    switch (standby.ResourceType)
                                    {
                                        case Models.Standbys.ResourceTypeEnum.Image:
                                            ShowImage(standby);
                                            break;
                                        case Models.Standbys.ResourceTypeEnum.Video:
                                            ShowVideo(standby);
                                            break;
                                        default: break;
                                    }
                }
                Show?.Invoke(false);
                _timer.Start();
            });
        }
        private void HiddenAll()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (FrameworkElement item in grid.Children)
                    item.Visibility = Visibility.Hidden;
            });
        }
        private static bool Download(Models.Standbys.Standby standby,out FileInfo file)
        {
            //var filename = new Uri(standby.ResourceUrl!).Segments.Last();
            //var ext = System.IO.Path.GetExtension(filename);
            file = new FileInfo($"./Cache/{standby.Id}{standby.Name}");
            return file.GetDownloadStatus(standby.ResourceUrl);
        }
        ~PlayPage() {
            _timer.Stop();
            _timer.Close();
        }
    }
}
