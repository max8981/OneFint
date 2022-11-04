using ClientLibrary.Enums;
using ClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Windows.Media.Animation;
using System.IO;
using Microsoft.Web.WebView2.Wpf;

namespace Client_Wpf
{
    /// <summary>
    /// ExhibitionComponent.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionComponent : UserControl,ClientLibrary.UIs.IExhibition
    {
        private readonly Dictionary<int, FrameworkElement> _elements;
        private MediaPlayer _mediaPlayer;
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
            _mediaPlayer = null!;
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
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        foreach (FrameworkElement element in grid.Children)
                            element.Visibility = Visibility.Hidden;
                    });
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Dispatcher.Invoke(() => grid.Background = new SolidColorBrush(Colors.Transparent));
        }

        public void ShowAudio(int id, string source)
        {
            Dispatcher.Invoke(() =>
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer
                    {
                        Volume = 1,
                    };
                    _mediaPlayer.MediaOpened += (o, e) => _mediaPlayer.Play();
                }
                _mediaPlayer.Open(new Uri(source));
            });
        }

        public void ShowClock(int id, BaseText? text, ClockTypeEnum clockType)
        {
            const string clockType1 = "yyyy-MM-dd ddd HH:mm:ss";
            const string clockType2 = "HH:mm:ss\nyyyy-MM-dd ddd";
            if (text != null)
            {
                text.Horizontal = (HorizontalEnum)2;
                text.Vertical = (VerticalEnum)2;
                var _clockFormat = clockType switch
                {
                    ClockTypeEnum.TYPE_1 => clockType1,
                    ClockTypeEnum.TYPE_2 => clockType2,
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
        }
        public void ShowDownload(int id, string title, string content, float progress)
        {
            var name = $"download{id}";
            DownloadControl downloader=
            Dispatcher.Invoke(() =>
            {
                Hidden();
                if (grid.FindName(name) is not DownloadControl downloader)
                {
                    downloader = new DownloadControl();
                    grid.Children.Add(downloader);
                    grid.RegisterName(name, downloader);
                    _elements.Add(id, downloader);
                }
                downloader.SetProgress(title, content, progress);
                downloader.Visibility = Visibility.Visible;
                return downloader;
            });
        }

        public void ShowImage(int id,string source)
        {
            Dispatcher.Invoke(() =>
            {
                Hidden();
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
                image.Source = GetImage(source);
                image.Visibility = Visibility.Visible;
            });
        }

        public void ShowText(int id, BaseText? text)
        {
            var name = $"text{id}";
            if(text!=null)
                Dispatcher.Invoke(() =>
                {
                    Hidden();
                    if (grid.FindName(name) is not TextBlock textBlock)
                    {
                        textBlock = new TextBlock
                        {
                            Name = name,
                            Margin = new Thickness(0, 0, 0, 0),
                            TextWrapping=TextWrapping.WrapWithOverflow,
                            //Background= new SolidColorBrush(Colors.Transparent),
                    };
                        grid.Children.Add(textBlock);
                        grid.RegisterName(name, textBlock);
                        _elements.Add(id, textBlock);
                    }
                    grid.Background = GetBrush(text.BackgroundColor);
                    textBlock.Text = text.Text;
                    textBlock.FontSize = GetFontSize(text.FontSize);
                    textBlock.Foreground = GetBrush(text.FontColor);
                    textBlock.TextAlignment = GetTextAlignment((int)text.Horizontal);
                    textBlock.HorizontalAlignment = GetHorizontal((int)text.Horizontal);
                    textBlock.VerticalAlignment = GetVertical((int)text.Vertical);
                    textBlock.Visibility = Visibility.Visible;
                });
        }

        public void ShowVideo(int id,string source,bool mute)
        {
            Dispatcher.Invoke(() =>
            {
                Hidden();
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
                    mediaElement.MediaEnded += (o, e) => mediaElement.Visibility = Visibility.Hidden;
                    grid.Children.Add(mediaElement);
                    grid.RegisterName("video", mediaElement);
                    _elements.Add(id, mediaElement);
                }
                mediaElement.Source = new Uri(source);
                mediaElement.Volume = mute ? 0 : 1;
                mediaElement.Stop();
                mediaElement.Play();
                mediaElement.Visibility = Visibility.Visible;
            });
        }

        public void ShowWeb(int id,string? url)
        {
            //ShowWebView2(id, url);
            //return;
            Dispatcher.BeginInvoke(() =>
            {
                Hidden();
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
                    webBrowser.Navigated += (o, e) => webBrowser.Visibility = Visibility.Visible;
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
        public void ShowWebView2(int id,string? url)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Hidden();
                if (grid.FindName("web") is not WebView2 webBrowser)
                {
                    webBrowser = new();
                    grid.Children.Add(webBrowser);
                    grid.RegisterName("web", webBrowser);
                    webBrowser.Loaded += (o, e) => webBrowser.Visibility = Visibility.Visible;
                    _elements.Add(id, webBrowser);
                }
                if ((string)webBrowser.Tag != url)
                {
                    webBrowser.Source = new Uri(url);
                    webBrowser.Tag = url;
                }
                webBrowser.Visibility = Visibility.Visible;
            });
        }
        private static Brush? GetBrush(string? colorString)
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
        private static double GetTextDisplayWidth(TextBlock text)
        {
            var str = text.Text;
            var fontFamily = text.FontFamily;
            var fontStyle = text.FontStyle;
            var fontWeight = text.FontWeight;
            var fontStretch = text.FontStretch;
            var FontSize = text.FontSize;
            var formattedText = new FormattedText(
                                str,
                                CultureInfo.CurrentUICulture,
                                FlowDirection.LeftToRight,
                                new Typeface(fontFamily, fontStyle, fontWeight, fontStretch),
                                FontSize,
                                Brushes.Black,
                                1);
            Size size = new Size(formattedText.Width, formattedText.Height);
            return size.Width;
        }
        private static BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using Stream ms = new MemoryStream(File.ReadAllBytes(imagePath));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;
        }
    }
}
