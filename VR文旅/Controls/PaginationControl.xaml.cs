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

namespace VR文旅.Controls
{
    /// <summary>
    /// PaginationControl.xaml 的交互逻辑
    /// </summary>
    public partial class PaginationControl : UserControl
    {
        public static readonly RoutedEvent PageChangedEvent = EventManager.RegisterRoutedEvent("PageChangedEvent", RoutingStrategy.Direct, typeof(RoutedPropertyChangedEventArgs<int>), typeof(PaginationControl));
        public event RoutedEventHandler PageChanged
        {
            //将路由事件添加路由事件处理程序
            add { AddHandler(PageChangedEvent, value); }
            //从路由事件处理程序中移除路由事件
            remove { RemoveHandler(PageChangedEvent, value); }
        }
        public PaginationControl()
        {
            InitializeComponent();
            panel.SizeChanged += (o, e) =>
            {
                FontSize = panel.ActualHeight/2;
                ItemWidth = (int)panel.ActualHeight;
            };
            //Draw();
        }
        private void Draw()
        {
            panel.Children.Clear();
            panel.Children.Add(GetLastPageButton());
            var start = CurrentPage < PagerCount ? 1 : CurrentPage - PagerCount / 2;
            start = PageCount + 1 - CurrentPage < PagerCount ? PageCount + 1 - PagerCount : start;
            var end = start + PagerCount - 1;
            if (start > 1)
            {
                panel.Children.Add(GetPageButton(1));
                panel.Children.Add(GetPageEllipsisLabel());
                if (end < PageCount)
                {
                    start++;
                    end--;
                }
            }
            else
                start = 1;
            for (int i = start; i <= end; i++)
            {
                panel.Children.Add(GetPageButton(i));
            }
            if (end < PageCount) {
                panel.Children.Add(GetPageEllipsisLabel());
                panel.Children.Add(GetPageButton(PageCount));
            } 
            panel.Children.Add(GetNextPageButton());
            panel.Children.Add(GetPageCountLabel(PageCount));
            if (PageCount > 1)
                Visibility = Visibility.Visible;
            else
                Visibility = Visibility.Hidden;
        }
        #region 元素宽度
        /// <summary>
        /// 每个元素的宽度
        /// </summary>
        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(int), typeof(PaginationControl), new PropertyMetadata(10, InitPagination));

        #endregion

        #region 每页显示条目个数
        /// <summary>
        /// 每页显示条目个数
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register("PageSize", typeof(int), typeof(PaginationControl), new PropertyMetadata(10, InitPagination));

        #endregion

        #region 总页数
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get { return (int)GetValue(PageCountProperty); }
            set { SetValue(PageCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageCountProperty =
            DependencyProperty.Register("PageCount", typeof(int), typeof(PaginationControl), new PropertyMetadata(0, InitPagination));
        #endregion

        #region 总条目数
        /// <summary>
        /// 总条目数
        /// </summary>
        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Total.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(int), typeof(PaginationControl), new PropertyMetadata(0, InitPagination));
        #endregion

        #region 当前页数
        /// <summary>
        /// 当前页数
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register("CurrentPage", typeof(int), typeof(PaginationControl), new PropertyMetadata(1, InitPagination));
        #endregion

        #region 页码按钮的数量，当总页数超过该值时会折叠  大于等于 5 且小于等于 21 的奇数


        public int PagerCount
        {
            get { return (int)GetValue(PagerCountProperty); }
            set { SetValue(PagerCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PagerCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PagerCountProperty =
            DependencyProperty.Register("PagerCount", typeof(int), typeof(PaginationControl), new PropertyMetadata(7, InitPagination), OnPagerCountPropertyValidate);

        private static bool OnPagerCountPropertyValidate(object value)
        {
            return int.TryParse(value?.ToString(), out int num) && (num & 1) != 0 && num >= 5 && num <= 21;
        }


        #endregion

        #region 只有一页时是否隐藏


        public bool HideOnSinglePage
        {
            get { return (bool)GetValue(HideOnSinglePageProperty); }
            set { SetValue(HideOnSinglePageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HideOnSinglePage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HideOnSinglePageProperty =
            DependencyProperty.Register("HideOnSinglePage", typeof(bool), typeof(PaginationControl), new PropertyMetadata(true, HideOnSinglePagePropertyChanged));

        private static void HideOnSinglePagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Pagination pagination)
            {
                pagination.SetCurrentValue(VisibilityProperty, (pagination.PageCount == 1 && (bool)e.NewValue) ? Visibility.Collapsed : Visibility.Visible);
            }
        }


        #endregion
        private static void InitPagination(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PaginationControl pagination)
            {
                if (e.Property == CurrentPageProperty)
                {
                    var pageChanged = new RoutedPropertyChangedEventArgs<int>((int)e.OldValue, (int)e.NewValue, PageChangedEvent);
                    pagination.RaiseEvent(pageChanged);
                    if (pageChanged.Handled)
                    {
                        return;
                    }
                }
                pagination.Draw();
            }
        }
        private Button GetLastPageButton()
        {
            Button button = new()
            {
                Content ="<",
                IsEnabled = CurrentPage > 1,
                Width = ItemWidth,
            };
            button.Click += (o, e) => CurrentPage--;
            return button;
        }
        private Button GetNextPageButton()
        {
            Button button = new()
            {
                Content = ">",
                IsEnabled = CurrentPage < PageCount,
                Width = ItemWidth,
            };
            button.Click += (o, e) => CurrentPage++;
            return button;
        }
        private Button GetPageButton(int page)
        {
            Button button = new()
            {
                Content = page,
                IsEnabled = page != CurrentPage,
                Width = ItemWidth,
            };
            button.Click += (o, e) => CurrentPage = page;
            return button;
        }
        private Label GetPageEllipsisLabel()
        {
            Label label = new()
            {
                Content = "...",
                HorizontalContentAlignment= HorizontalAlignment.Center,
                VerticalContentAlignment= VerticalAlignment.Center,
                Width = ItemWidth,
            };
            return label;
        }
        private static Label GetPageCountLabel(int count)
        {
            Label label = new()
            {
                Content = $"共{count}页",
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            return label;
        }
        public void SetPage(int page)
        {
            CurrentPage = Math.Clamp(page, 0, PageCount);
        }
    }
}
