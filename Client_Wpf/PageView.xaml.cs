using ClientLibrary.UIs;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client_Wpf
{
    /// <summary>
    /// PageView.xaml 的交互逻辑
    /// </summary>
    public partial class PageView : Window, ClientLibrary.UIs.IPageController
    {
        public PageView()
        {
            InitializeComponent();
            grid.Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Activated += (o, e) => WindowsController.ShowCursor();
            Deactivated += (o, e) => WindowsController.HiddenCursor();

#if DEBUG
            Topmost = false;
#endif
        }

        public int Id { get; set; }
        public IExhibition TryAddExhibition(int id, string name, System.Drawing.Rectangle rectangle, int z)
        {
            ExhibitionComponent exhibition =
            Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not ExhibitionComponent element)
                {
                    element = new ExhibitionComponent(id, name, rectangle, z);
                    grid.RegisterName(name, element);
                    grid.Children.Add(element);
                }
                Panel.SetZIndex(this, z);
                return element;
            });
            return exhibition;
        }

        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                machineCodeLabel.Visibility = Visibility.Hidden;
                //foreach (UserControl control in grid.Children)
                //{
                //    grid.UnregisterName(control.Name);
                //    grid.Children.Remove(control);
                //}
            });
        }

        public bool TryFindExhibition(string name, out IExhibition? exhibition)
        {
            bool result = false;
            exhibition =
            Dispatcher.Invoke(() =>
            {
                foreach (IExhibition item in grid.Children)
                {
                    if (item.Name == name)
                    {
                        result = true;
                        return item;
                    }
                }
                return null;
            });
            return result;
        }

        public bool TryFindExhibition(int id, out IExhibition? exhibition)
        {
            bool result = false;
            exhibition =
            Dispatcher.Invoke(() =>
            {
                foreach (IExhibition item in grid.Children)
                {
                    if (item.Id == id)
                    {
                        result = true;
                        return item;
                    }
                }
                return null;
            });
            return result;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Visibility = Visibility.Hidden;
                    break;
            }
        }

        public void ShowView() => Dispatcher.Invoke(() => Show());

        public void ShowCode(string code)
        {
            Dispatcher.Invoke(() => machineCodeLabel.Content = code);
        }
    }
}
