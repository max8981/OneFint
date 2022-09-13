using ClientLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// View.xaml 的交互逻辑
    /// </summary>
    public partial class View : Window
    {
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern void ShowCursor(int status);
        private bool xianshiqi;
        private int volume = 0;
        private int brightness = 50;
        public View()
        {
            InitializeComponent();
            Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Activated += (o, e) => { ShowCursor(0); };
            Deactivated+=(o,e) => { ShowCursor(1); };
        }
        public void AddControl(string name,UserControl control)
        {
            grid.RegisterName(name, control);
            grid.Children.Add(control);
        }
        public void RemoveControl(string name)
        {
            var control = grid.FindName(name) as UIElement;
            grid.Children.Remove(control);
        }
        public ExhibitionControl AddElement(string name, int w, int h, int x, int y, int z)
        {
            ExhibitionControl element =
            Dispatcher.Invoke(() =>
            {
                if (grid.FindName(name) is not ExhibitionControl element)
                {
                    element = new ExhibitionControl(name, w, h, x, y, z);
                    grid.RegisterName(name, element);
                    grid.Children.Add(element);
                }
                Panel.SetZIndex(this, z);
                return element;
            });
            return element;
        }
        public void SetLayout(ExhibitionControl[] controls)
        {
            foreach (var control in controls)
            {
                grid.RegisterName(control.Name, control);
                grid.Children.Add(control);
            }
        }
        public void Clear()
        {
            Dispatcher.Invoke(() =>
            {
                grid.Children.Clear();
            });
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Add:
                    volume = volume < 100 ? volume + 1 : volume;
                    ClientController.SetVolume(volume);
                    break;
                case Key.Subtract:
                    volume = volume > 0 ? volume - 1 : volume;
                    ClientController.SetVolume(volume);
                    break;
                case Key.Escape:
                    Visibility = Visibility.Hidden;
                    break;
                case Key.Space:
                    if (!xianshiqi)
                    {
                        ClientController.ScreenPowerOff(0);
                        xianshiqi = true;
                    }
                    else
                    {
                        ClientController.ScreenPowerOn(0);
                        xianshiqi = false;
                    }
                    break;
                case Key.Up:
                    brightness = brightness < 100 ? brightness + 1 : brightness;
                    ClientController.ScreenBrightness(brightness);
                    break;
                case Key.Down:
                    brightness = brightness > 0 ? brightness - 1 : brightness;
                    ClientController.ScreenBrightness(brightness--);
                    break;
            }
        }
        public void ShowView()
        {
            Dispatcher.Invoke(() => Visibility = Visibility.Visible);
        }
    }
}
