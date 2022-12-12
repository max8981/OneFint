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
using System.Windows.Shapes;

namespace VR文旅
{
    /// <summary>
    /// ViewWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ViewWindow : Window
    {
        public ViewWindow()
        {
            InitializeComponent();
            Left = 0;
            Top = 0;
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            base.Width = SystemParameters.PrimaryScreenWidth;
            base.Height = SystemParameters.PrimaryScreenHeight + 400;
        }
        public void Show(string url)
        {
            var wbrReport = new CefSharp.Wpf.ChromiumWebBrowser();
            wbrReport.HorizontalAlignment = HorizontalAlignment.Stretch;
            wbrReport.VerticalAlignment = VerticalAlignment.Stretch;
            grid.Children.Add(wbrReport);
            //wbrReport.Address = "https://www.720yun.com/t/83cjegevuw2?scene_id=17199073";
            wbrReport.Address = url;
            var button = new Button();
            button.Content = "返回";
            button.Margin = new Thickness(10, 10, 0, 0);
            button.Width = 80;
            button.Height = 80;
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Background = new SolidColorBrush(Colors.Transparent);
            button.Click += (o, e) =>
            {
                this.Visibility = Visibility.Hidden;
            };
            button.Visibility = Visibility.Hidden;
            grid.Children.Add(button);
            Panel.SetZIndex(button, grid.Children.Count + 1);
            var image = new Image();
            image.HorizontalAlignment = HorizontalAlignment.Right; 
            image.VerticalAlignment = VerticalAlignment.Bottom;
            image.Margin = new Thickness(0, 0, 20, 10);
            image.Width = 300;
            image.Height = 100;
            image.Source = new BitmapImage(new Uri("https://tse2-mm.cn.bing.net/th/id/OIP-C.9uxRZpk8QSXmZKPOY5m43QHaCb?pid=ImgDet&rs=1"));
            grid.Children.Add(image);
            Panel.SetZIndex(image, grid.Children.Count + 1);
            wbrReport.Loaded += (o, e) => Dispatcher.Invoke(() => button.Visibility = Visibility.Visible);
            this.Show();
        }
    }
}
