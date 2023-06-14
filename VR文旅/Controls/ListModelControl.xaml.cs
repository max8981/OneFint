using System;
using System.Collections.Generic;
using System.Data;
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
    /// ListModelControl.xaml 的交互逻辑
    /// </summary>
    public partial class ListModelControl : UserControl
    {
        public event Action<string?>? Selected;
        private int _page = 0;
        private double _lastWidth = 0;
        public ListModelControl()
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) =>
            {
                if (_lastWidth != grid.ActualWidth)
                {
                    var rowHeight = grid.ActualWidth / 20;
                    var itemWidth = grid.ActualHeight / 3;
                    var itemCount = grid.ActualHeight / 10;
                    ItemCount = (int)(panel.ActualHeight / rowHeight);
                    _lastWidth = grid.ActualWidth;
                    //Draw();
                }
            };
            //dataGrid.MouseLeftButtonDown += (o, e) => ShowView();
            Models.PLayLists.PlayListChanged += o =>
            {
                _page = 0;
                Draw();
            };
        }
        public int ItemCount
        {
            get { return (int)GetValue(ItemCountProperty); }
            set { SetValue(ItemCountProperty, value); }
        }
        public static readonly DependencyProperty ItemCountProperty =
            DependencyProperty.Register("ItemCount", typeof(int), typeof(ListModelControl), new PropertyMetadata(10, InitPagination));
        private static void InitPagination(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListModelControl pagination)
                pagination.Draw();
        }
        internal void Draw()
        {
            var data = Models.PLayLists.GetScenarios(_page, ItemCount);
            var c = (double)Models.PLayLists.Count / ItemCount;
            pagination.PageCount = (int)Math.Ceiling(c);
            pagination.PageSize = ItemCount;
            panel.Children.Clear();
            for (int i = 0; i < data.Length; i++)
            {
                var row = new ListRowControl(data[i]);
                if (i % 2 != 0)
                    row.Background = new SolidColorBrush(Colors.White);
                row.Selected += Selected;
                panel.Children.Add(row);
            }
            if (pagination.CurrentPage != _page + 1)
                pagination.SetPage(_page + 1);
        }
        private void Pagination_PageChanged(object sender, RoutedEventArgs e)
        {
            if (e is System.Windows.RoutedPropertyChangedEventArgs<int> value)
                GetData(value.NewValue);
        }
        private void GetData(int page)
        {
            _page = page - 1;
            Draw();
        }
    }
}
