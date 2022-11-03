using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Client_VR.CustomControls
{
    /// <summary>
    /// ViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class ViewControl : UserControl
    {
        public ViewControl()
        {
            InitializeComponent();
            var wbrReport = new CefSharp.Wpf.ChromiumWebBrowser();
            grid.Children.Add(wbrReport);
            wbrReport.Address = "https://www.720yun.com/t/83cjegevuw2?scene_id=17199073";
            var button = new Button();
            button.Content = "返回";
            button.Margin = new Thickness(10, 10, 0, 0);
            button.Width = 40;
            button.Height = 40;
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
            wbrReport.Loaded += (o, e) => Dispatcher.Invoke(() => button.Visibility = Visibility.Visible);
        }
    }
}
