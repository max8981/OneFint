using Client_Wpf.CustomControls;
using ClientLibrary;
using ClientLibrary.Enums;
using ClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
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
using static ClientLibrary.Downloader;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Threading;
using ClientLibrary.UIs;
using System.Reflection;
using System.Drawing;
using System.Security.Policy;
using System.Timers;

namespace Client_Wpf
{
    /// <summary>
    /// ExhibitionComponent.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionComponent : UserControl,ClientLibrary.UIs.IExhibition
    {
        private readonly Dictionary<int, FrameworkElement> _elements;
        public ExhibitionComponent(int id, string name, System.Drawing.Rectangle rectangle, int z)
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
            _elements = new();
            Panel.SetZIndex(this, z);
        }

        public int Id { get; init; }

        public void Hidden(int id = -1)
        {
            if (id > 0)
            {
                if(_elements.TryGetValue(id, out var element))
                    Dispatcher.Invoke(() => element.Visibility = Visibility.Hidden);
            }
            else
            {
                foreach (FrameworkElement element in grid.Children)
                {
                    Dispatcher.Invoke(() => element.Visibility = Visibility.Hidden);
                }
            }
        }

        public void ShowAudio(int id, string source)
        {
            throw new NotImplementedException();
        }

        public void ShowClock(int id, BaseText text, ClockTypeEnum clockType)
        {
            const string clockType1 = "yyyy-MM-dd ddd HH:mm:ss";
            const string clockType2 = "HH:mm:ss\nyyyy-MM-dd ddd";
            //var clock = Dispatcher.Invoke(() =>
            //{
            //    TextBlock clock = new()
            //    {
            //        Margin = new Thickness(0, 0, 0, 0),
            //        Text = "clock",
            //        FontSize = size,
            //        Foreground = GetBrush(fore),
            //        Background = GetBrush(back),
            //        TextAlignment = GetTextAlignment(2),
            //        HorizontalAlignment = GetHorizontal(2),
            //        VerticalAlignment = GetVertical(2)
            //    };
            //    grid.Children.Add(clock);
            //    grid.Background = GetBrush(back);
            //    return clock;
            //});
            var _clockFormat = clockType switch
            {
                ClockTypeEnum.TYPE_1=>clockType1,
                ClockTypeEnum.TYPE_2=>clockType2,
                _ => "",
            };
            System.Timers.Timer timer = new()
            {
                AutoReset = true,
                Interval = 1000
            };
            timer.Elapsed += (o, e) => {
                text.Text = e.SignalTime.ToString(_clockFormat);
                ShowText(id, text);
            };
            timer.Start();
        }
        public void ShowDownload(int id, string title, string content, float progress)
        {
            var name = $"download{id}";
            DownloadControl downloader=
            Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not DownloadControl downloader)
                {
                    downloader = new DownloadControl();
                    grid.Children.Add(downloader);
                    grid.RegisterName(name, downloader);
                    _elements.Add(id, downloader);
                }
                downloader.SetProgress(title, content, progress);
                return downloader;
            });
        }

        public void ShowImage(int id,string source)
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
                    _elements.Add(id, image);
                }
                image.Source = new BitmapImage(new Uri(source));
                image.Visibility = Visibility.Visible;
            });
        }

        public void ShowText(int id, BaseText text)
        {
            var name = $"text{id}";
            Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not TextBlock textBlock)
                {
                    textBlock = new TextBlock
                    {
                        Name = name,
                        Margin = new Thickness(0, 0, 0, 0),
                    };
                    grid.Children.Add(textBlock);
                    grid.RegisterName(name, textBlock);
                    _elements.Add(id, textBlock);
                }
                textBlock.Text = text.Text;
                textBlock.FontSize = GetFontSize(text.FontSize);
                textBlock.Foreground = GetBrush(text.FontColor);
                textBlock.Background = GetBrush(text.BackgroundColor);
                textBlock.TextAlignment = GetTextAlignment((int)text.Horizontal);
                textBlock.HorizontalAlignment = GetHorizontal((int)text.Horizontal);
                textBlock.VerticalAlignment = GetVertical((int)text.Vertical);
                grid.Background = GetBrush(text.BackgroundColor);
            });
        }

        public void ShowVideo(int id,string source,bool mute)
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
                        Stretch = Stretch.Fill,
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
                    _elements.Add(id, mediaElement);
                }
                mediaElement.Visibility = Visibility.Visible;
                mediaElement.Source = new Uri(source);
                mediaElement.Volume = mute ? 0 : 1;
                mediaElement.Play();
            });
        }

        public void ShowWeb(int id,string url)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (grid.FindName("web") is not WebBrowser webBrowser)
                {
                    webBrowser = new();
                    grid.Children.Add(webBrowser);
                    grid.RegisterName("web", webBrowser);
                    var script = "document.body.style.overflow='hidden'";
                    webBrowser.LoadCompleted += (o, e) =>
                    {
                        webBrowser.InvokeScript("execScript", new object[] { script, "JavaScript" });
                    };
                    //webBrowser.Navigated += (o, e) => webBrowser.Visibility = Visibility.Visible;
                    _elements.Add(id, webBrowser);
                }
                if ((string)webBrowser.Tag != url)
                {
                    webBrowser.Navigate(url);
                    webBrowser.Tag = url;
                }
                webBrowser.Visibility = Visibility.Visible;
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
        private static Brush? GetBrush(string colorString)
        {
            if (string.IsNullOrEmpty(colorString))
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
                return new SolidColorBrush(color);
            }
        }
        private static double GetFontSize(int? value)
        {
            return value == null ? 14d : (double)value;
        }
        private static System.Windows.HorizontalAlignment GetHorizontal(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.HorizontalAlignment.Left,
                2 => System.Windows.HorizontalAlignment.Center,
                3 => System.Windows.HorizontalAlignment.Right,
                _ => System.Windows.HorizontalAlignment.Stretch,
            };
        }
        private static System.Windows.VerticalAlignment GetVertical(int vertical)
        {
            return vertical switch
            {
                1 => System.Windows.VerticalAlignment.Top,
                2 => System.Windows.VerticalAlignment.Center,
                3 => System.Windows.VerticalAlignment.Bottom,
                _ => System.Windows.VerticalAlignment.Stretch,
            };
        }
        private static System.Windows.TextAlignment GetTextAlignment(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.TextAlignment.Left,
                2 => System.Windows.TextAlignment.Center,
                3 => System.Windows.TextAlignment.Right,
                _ => System.Windows.TextAlignment.Justify,
            };
        }
    }
}
