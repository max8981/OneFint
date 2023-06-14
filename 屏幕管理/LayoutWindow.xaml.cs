using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace 屏幕管理
{
    /// <summary>
    /// LayoutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutWindow : Window,Interfaces.ILayoutWindow
    {
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        private static extern void ShowCursor(int status);
        public LayoutWindow(System.Windows.Forms.Screen screen)
        {
            InitializeComponent();
            grid.Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Left = screen.Bounds.X;
            Top = screen.Bounds.Y;
            base.Width = SystemParameters.PrimaryScreenWidth;
            base.Height = SystemParameters.PrimaryScreenHeight;
            Activated += (o, e) => ShowCursor(0);
            Deactivated += (o, e) => ShowCursor(1);
#if DEBUG
            Topmost = false;
#endif
            this.Activate();
            this.Focus();
        }
        public int Id { get; set; }
        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (UserControl item in grid.Children)
                    grid.UnregisterName(item.Name);
                grid.Children.Clear();
            });
        }
        public System.Drawing.Size GetSize() => Dispatcher.Invoke(() => new System.Drawing.Size((int)ActualWidth, (int)ActualHeight));
        public async void ShowMessage(string message, int delay)
        {
            await Dispatcher.Invoke(async () =>
            {
                var name = typeof(RichTextBox).Name;
                if (grid.FindName(name) is not System.Windows.Controls.RichTextBox richTextBox)
                {
                    richTextBox = new System.Windows.Controls.RichTextBox
                    {
                        Name = name,
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };
                    richTextBox.Document = new FlowDocument();
                    grid.RegisterName(name, richTextBox);
                    grid.Children.Add(richTextBox);
                    System.Windows.Controls.Panel.SetZIndex(this, 99);
                }
                var block = new Paragraph(new Run(message) { Foreground = new SolidColorBrush(Colors.Gray) { Opacity = 0.6 } });
                richTextBox.Document.Blocks.Add(block);
                richTextBox.ScrollToEnd();
                await Task.Delay(delay * 1000);
                richTextBox.Document.Blocks.Remove(block);
                return richTextBox;
            });
        }
        public void ShowView(double width,double height) {
            Dispatcher.Invoke(() =>
            {
                base.Width = width;
                base.Height = height;
                Show();
                Activate();
                Focus();
            });
            ShowCursor(0);
        }
        public void ShowView()
        {
            Dispatcher.Invoke(() =>
            {
                Show();
                Activate();
                Focus();
            });
            ShowCursor(0);
        }
        public void ShowCode(string code)
        {
            Dispatcher.Invoke(() =>
            {
                grid.Children.Add(new TextBlock()
                {
                    Text = code,
                    FontSize = 144,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White),
                });
                grid.Background = new SolidColorBrush(Colors.Black);
            });
        }
        public void HiddenCode()
        {
            Dispatcher.Invoke(() => Hide());
        }
        public Controllers.ExhibitionController TryAddExhibition(int id, string name, System.Drawing.Rectangle rectangle, int z)
        {
            var exhibition = Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not ExhibitionControl control)
                {
                    control = new ExhibitionControl(id, name, rectangle);
                    Panel.SetZIndex(control, z);
                    grid.RegisterName(name, control);
                    grid.Children.Add(control);
                }
                System.Windows.Controls.Panel.SetZIndex(this, z);
                return control;
            });
            return new Controllers.ExhibitionController(exhibition);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Visibility = Visibility.Hidden;
                    ShowCursor(1);
                    break;
            }
        }
    }
}
