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
        public event Action<string?>? Selected;
        private readonly Dictionary<int, ImageModelView> _imageModelViews = new();
        private int _page = 0;
        private double _itemWidth;
        private int _maxPage = 0;
        private Point _mp;
        private static bool _mm;
        public static bool Move => _mm;
        public ImageModelControl()
        {
            InitializeComponent();
            //left.PreviewMouseLeftButtonUp += (o, e) => PrevPage();
            //leftImage.Source = Global.GetBitmap("左翻页");
            //right.PreviewMouseLeftButtonUp += (o, e) => NextPage();
            //rightImage.Source = Global.GetBitmap("右翻页");
            grid.SizeChanged += (o, e) =>
            {
                _itemWidth = grid.ActualWidth / 4.5;
                ItemCount = 999;
            };
            Models.PLayLists.PlayListChanged += x => Draw();
            grid.PreviewMouseDown += (o, e) =>
            {
                _mm = false;
                _mp = Global.GetMousePoint();
            };
            grid.PreviewMouseMove += (o, e) =>
            {
                if (e.LeftButton==MouseButtonState.Pressed)
                {
                    var mp = Global.GetMousePoint();
                    if (mp.X > (_mp.X + 10))
                    {
                        _mp = mp;
                        scroll.LineLeft();
                        _mm = true;
                    }
                    else if (mp.X < (_mp.X - 10))
                    {
                        _mp = mp;
                        scroll.LineRight();
                        _mm = true;
                    }
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
            GetData(_page);
        }
        public void NextPage()
        {
            //scroll.LineRight();
            _page++;
            _page = _page < _maxPage ? _page : 0;
            //Draw();
        }
        public void PrevPage()
        {
            //scroll.LineLeft();
            _page = _page > 0 ? _page - 1 : _maxPage - 1;
            //Draw();
        }
        private void GetData(int page)
        {
            panel.Children.Clear();
            _imageModelViews.Clear();
            _maxPage = (int)Math.Ceiling((double)Models.PLayLists.Count / ItemCount);
            var data = Models.PLayLists.GetScenarios(page, ItemCount);
            for (int i = 0; i < data.Length; i++)
            {
                var view = new ImageModelView(data[i])
                {
                    Id = i,
                    Width = _itemWidth,
                    Margin = new(0, 0, 0, 0)
                };
                view.Selected += Selected;
                _imageModelViews.Add(i, view);
                panel.Children.Add(view);
            }
            if (_maxPage > 1)
            {
                left.Visibility = Visibility.Visible;
                right.Visibility = Visibility.Visible;
            }
            else
            {
                left.Visibility = Visibility.Hidden;
                right.Visibility = Visibility.Hidden;
            }
        }
    }
}
