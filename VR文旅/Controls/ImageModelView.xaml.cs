using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VR文旅.Controls
{
    /// <summary>
    /// ImageModelView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageModelView : UserControl
    {
        private Size _size;
        private Size _maxsize;
        private Size _minsize;
        private const double RATIO = 1.5;
        public event Action<string?>? Selected;
        private static IInputElement? _element;
        public int Id { get; set; }
        internal ImageModelView(Models.Scenario scenario)
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) =>
            {
                if (Height > 0)
                {
                    grid.Width = Height / RATIO;
                    grid.Height = Height;
                }
                else if (Width > 0)
                {
                    grid.Height = Width * RATIO;
                    grid.Width = Width;
                }
                _maxsize = new Size(grid.Width * 0.75, grid.Height * 0.75);
                _minsize = new Size(grid.Width * 0.70, grid.Height * 0.70);
                scenarioName.FontSize = Math.Clamp(grid.ActualWidth / 12, 12, 48);
                description.FontSize = grid.ActualHeight / 16;
                border.Margin = new(0, 60, 0, 0);
                border.Width = _minsize.Width;
                border.Height = _minsize.Height;
                background.Margin = new(0, 60, 0, 0);
                background.Width = _minsize.Width;
                background.Height = _minsize.Height;
            };
            thumb.ImageSource = Global.GetBitmap("Logo");
            scenarioName.Text = scenario.ScenarioName;
            scenarioName.Foreground=new SolidColorBrush(Colors.White);
            description.Text = scenario.Description;
            var polygon = new Controls.PolygonControl($"{scenario.Province},{scenario.City}")
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Bottom,
                Margin = new(0, 0, 0, 0),
                FontSize = 30,
            };
            background.Child = polygon;
            background.Visibility = Visibility.Hidden;
            DelayedLoading(scenario);
            dpd.AddValueChanged(grid, (o, e) => {
                if (IsMouseOver)
                    Enter(null, null);
                else
                    SelectCancel();
            });
            grid.PreviewMouseLeftButtonDown += (o, e) =>
            {
                _element = e.Device.Target;
            };
            grid.PreviewMouseLeftButtonUp += (o, e) =>
            {
                if (!ImageModelControl.Move&& e.Device.Target.Equals(_element))
                    Selected?.Invoke(scenario.WebLink);
            };
        }
        public void SelectCancel()
        {
            _size = new Size(border.Width, border.Height);
            border.Margin = new(0, 60, 0, 0);
            border.Width = _minsize.Width;
            border.Height = _minsize.Height;
            background.Margin = new(0, 60, 0, 0);
            background.Width = _minsize.Width;
            background.Height = _minsize.Height;
            background.Visibility = Visibility.Hidden;
        }
        private readonly DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(Grid.IsMouseOverProperty, typeof(Grid));  
        private async void Enter(object? sender, RoutedEventArgs? e)
        {
            while (border.Width < _maxsize.Width)
            {
                Dispatcher.Invoke(() =>
                {
                    if (grid.IsMouseOver)
                    {
                        _size = new Size(border.Width, border.Height);
                        border.Height = _size.Height * 1.03;
                        border.Width = border.Height / RATIO;
                        background.Margin = new Thickness(0, background.Margin.Top + RATIO * 5, 0, 0);
                        background.Height = _size.Height * 1.1;
                        background.Width = background.Height / RATIO;
                        background.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SelectCancel();
                        return;
                    }

                });
                await Task.Delay(40);
            }
        }
        private async void DelayedLoading(Models.Scenario scenario)
        {
            thumb.ImageSource = await scenario.GetBitmapAsync();
            thumb.Stretch = Stretch.UniformToFill;
        }
    }
}
