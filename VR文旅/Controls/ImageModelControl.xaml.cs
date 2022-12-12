using System;
using System.Collections.Generic;
using System.IO;
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

namespace VR文旅.Controls
{
    /// <summary>
    /// ImageModelControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageModelControl : UserControl
    {
        private int _page = 1;
        private double _lastWidth = 0;
        public ImageModelControl()
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) =>
            {
                if (_lastWidth != grid.ActualWidth)
                {
                    var itemWidth = grid.ActualHeight / 3;
                    var itemCount = grid.ActualWidth / itemWidth;
                    ItemCount = (int)itemCount - 1;
                    _lastWidth= grid.ActualWidth;
                }
            };
        }
        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register("ItemCount", typeof(int), typeof(ImageModelControl), new PropertyMetadata(10, InitPagination));
        private static void InitPagination(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageModelControl pagination)
                pagination.Draw();
        }
        private void Draw()
        {
            grid.Children.Add(GetLeftButton());
            grid.Children.Add(GetRightButton());
            GetData(_page);
        }
        private Image GetLeftButton()
        {
            Image image = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Source = Global.GetBitmap("Left"),
            };
            image.MouseLeftButtonDown += (o, e) =>
            {
                if (_page > 1)
                {
                    _page--;
                    Draw();
                }
            };
            Grid.SetColumn(image, 0);
            return image;
        }
        private Image GetRightButton()
        {
            Image image = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Source = Global.GetBitmap("Right"),
            };
            image.MouseLeftButtonDown += (o, e) =>
            {
                _page++;
                Draw();
            };
            Grid.SetColumn(image, 2);
            return image;
        }
        private void GetData(int page)
        {
            panel.Children.Clear();
            var data=Models.Fake.GetPlayList(ItemCount, page);
            for (int i = 0; i < data.Scenarios.Length; i++)
            {
                panel.Children.Add(new ImageModelView(data.Scenarios[i]));
            }
        }
    }
}
