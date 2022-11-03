using ClientLibrary.Models;
using ClientLibrary.UIs;
using PP.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Client_Wpf
{
    /// <summary>
    /// PageView.xaml 的交互逻辑
    /// </summary>
    public partial class PageView : Window, ClientLibrary.UIs.IPageController
    {
        public PageView(System.Windows.Forms.Screen screen)
        {
            InitializeComponent();
            grid.Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Left = screen.Bounds.X;
            Top = screen.Bounds.Y;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Activated += (o, e) => WindowsController.HiddenCursor();
            Deactivated += (o, e) => WindowsController.ShowCursor();
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
                System.Windows.Controls.Panel.SetZIndex(this, z);
                return element;
            });
            return exhibition;
        }

        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                machineCodeLabel.Visibility = Visibility.Hidden;
                foreach (dynamic control in grid.Children)
                {
                    grid.UnregisterName(control.Name);
                }
                grid.Children.Clear();
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

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

        public async void ShowMessage(string message, TimeSpan delay)
        {
            await Dispatcher.Invoke(async () =>
            {
                var name = "richTextBox";
                if (grid.FindName(name) is not System.Windows.Controls.RichTextBox richTextBox)
                {
                    richTextBox = new System.Windows.Controls.RichTextBox
                    {
                        Name = name,
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        HorizontalAlignment=System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment=VerticalAlignment.Stretch,
                    };
                    richTextBox.Document = new FlowDocument();
                    grid.RegisterName(name, richTextBox);
                    grid.Children.Add(richTextBox);
                    System.Windows.Controls.Panel.SetZIndex(this, 99);
                }
                var block = new Paragraph(new Run(message) { Foreground = new SolidColorBrush(Colors.Gray) { Opacity = 0.6 } });
                richTextBox.Document.Blocks.Add(block);
                richTextBox.ScrollToEnd();
                await Task.Delay(delay);
                richTextBox.Document.Blocks.Remove(block);
                return richTextBox;
            });
        }
    }
}
