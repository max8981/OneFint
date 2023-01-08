using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using System.Xml.Linq;

namespace VR文旅.Controls
{
    /// <summary>
    /// WebBrowserControl.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserControl : UserControl
    {
        public event Action? Return;
        public WebBrowserControl()
        {
            InitializeComponent();
        }
        public void Navigation(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;
            var name = typeof(CefSharp.Wpf.ChromiumWebBrowser).Name;
            var chromium = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not CefSharp.Wpf.ChromiumWebBrowser chromium)
                {
                    chromium = new();
                    chromium.IsVisibleChanged += (o, e) =>
                    {
                        var visible = e.NewValue as bool?;
                        if (visible == false)
                            chromium.ToggleAudioMute();
                        else
                            chromium.ToggleAudioMute();
                    };
                    chromium.HorizontalAlignment = HorizontalAlignment.Stretch;
                    chromium.VerticalAlignment = VerticalAlignment.Stretch;
                    grid.Children.Add(chromium);
                    var button = new Button
                    {
                        Content = "返回",
                        Margin = new Thickness(10, 10, 0, 0),
                        Width = 80,
                        Height = 80,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        Background = new SolidColorBrush(Colors.Transparent)
                    };
                    button.Click += (o, e) =>
                    {
                        Visibility = Visibility.Hidden;
                        Return?.Invoke();
                    };
                    //button.Visibility = Visibility.Hidden;
                    grid.Children.Add(button);
                    Panel.SetZIndex(button, grid.Children.Count + 1);
                    var image = new Image
                    {
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 20, 10),
                        Width = 300,
                        Height = 100,
                        Source = Global.GetBitmap("Logo")
                    };
                    grid.Children.Add(image);
                    Panel.SetZIndex(image, grid.Children.Count + 1);
                    //chromium.Loaded += (o, e) => Dispatcher.Invoke(() => button.Visibility = Visibility.Visible);
                    //chromium.Visibility = Visibility.Hidden;
                    //chromium.LoadingStateChanged += (o, e) =>
                    //{
                    //    if (e.IsLoading)
                    //        Dispatcher.Invoke(() => chromium.Visibility = Visibility.Visible);
                    //};
                    grid.RegisterName(name, chromium);
                }
                chromium.Visibility = Visibility.Visible;
                return chromium;
            });
            chromium.Address = url;
            Visibility = Visibility.Visible;
        }
    }
}
