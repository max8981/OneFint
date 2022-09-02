using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
using static Client_Wpf.CustomControls.ControlHelper;
using static System.Net.Mime.MediaTypeNames;

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// ExhibitionControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExhibitionControl : UserControl
    {
        Timer _timer = new Timer(1000)
        {
            AutoReset = true,
        };
        public ExhibitionControl()
        {
            InitializeComponent();
        }
        public ExhibitionControl(SharedProject.Component component)
        {
            InitializeComponent();
            Width = component.W;
            Height = component.H;
            Margin = new Thickness(component.X, component.Y, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment= VerticalAlignment.Top;
            grid.Background = GetBrush(component.BackgroundColor);
            if (!string.IsNullOrEmpty(component.Name))
                RegisterName(component.Name, this);
            Background = GetBrush(component.BackgroundColor);
        }
        public UserControl ShowText(SharedProject.BaseText text)
        {
            var textBlock = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 0),
                Text = text.Text,
                FontSize = text.FontSize == null ? 14d : (double)text.FontSize,
                Foreground = GetBrush(text.FontColor),
                Background = GetBrush(text.BackgroundColor),
                TextAlignment = GetTextAlignment((int)text.Horizontal),
                HorizontalAlignment = GetHorizontal((int)text.Horizontal),
                VerticalAlignment = GetVertical((int)text.Vertical)
            };
            grid.Children.Add(textBlock);
            return this;
        }
        public UserControl ShowClock(SharedProject.Component component)
        {
            const string clockType1 = "yyyy-MM-dd ddd HH:mm:ss";
            const string clockType2 = "HH:mm:ss\nyyyy-MM-dd ddd";
            var timer = new System.Timers.Timer(1000)
            {
                AutoReset = true,
            };
            var textBlock = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 0),
                FontSize = component.Text.FontSize == null ? 14d : (double)component.Text.FontSize,
                Foreground = GetBrush(component.Text.FontColor),
                Background = GetBrush(component.Text.BackgroundColor),
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(textBlock);
            timer.Elapsed += component.ClockType switch
            {
                SharedProject.Component.ClockTypeEnum.TYPE_1 => (o, e) =>
                {
                    Dispatcher.Invoke(() => { textBlock.Text = e.SignalTime.ToString(clockType1); });
                }
                ,
                SharedProject.Component.ClockTypeEnum.TYPE_2 => (o, e) =>
                {
                    Dispatcher.Invoke(() => { textBlock.Text = e.SignalTime.ToString(clockType2); });
                }
                ,
                _ => (o, e) =>
                {
                    Dispatcher.Invoke(() => { textBlock.Text = e.SignalTime.ToString(clockType1); });
                }
                ,
            };
            timer.Start();
            return this;
        }
        public UserControl ShowWebBrowser(SharedProject.BaseText text)
        {
            var webBrowser = new WebBrowser();
            grid.Children.Add(webBrowser);
            webBrowser.Navigate(text.Text);
            FieldInfo? fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if(fiComWebBrowser != null)
            {
                object? objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                if (objComWebBrowser != null)
                {
                    objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { true });
                }
            }
            return this;
        }
        public UserControl ShowImage(SharedProject.Content content)
        {
            BitmapImage img = new BitmapImage(new Uri(content.Material.Content));
            var image = new System.Windows.Controls.Image();
            image.Source = img;
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Children.Add(image);
            return this;
        }
        public UserControl ShowVideo(SharedProject.Content content)
        {
            MediaElement mediaElement = new MediaElement();
            mediaElement.VerticalAlignment = VerticalAlignment.Stretch;
            mediaElement.HorizontalAlignment = HorizontalAlignment.Stretch;
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.MediaEnded += (o, e) => { mediaElement.Stop(); mediaElement.Play(); };
            if (System.IO.File.Exists($"./download/{content.Name}"))
            {
                mediaElement.Source = new Uri($"./download/{content.Name}", UriKind.Relative);
                mediaElement.Play();
            }
            else
            {
                var download = new DownloadProgressControl.DownloadTask(content.Id, content.Material.Content, content.Name);
                download.Start(c => {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        mediaElement.Source = new Uri(c.FileInfo.FullName, UriKind.Relative);
                        mediaElement.Play();
                    }));
                });
            }
            grid.Children.Add(mediaElement);
            return this;
        }
    }
}
