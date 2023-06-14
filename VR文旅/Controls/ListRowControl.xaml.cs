using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VR文旅.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace VR文旅.Controls
{
    /// <summary>
    /// ListRowControl.xaml 的交互逻辑
    /// </summary>
    public partial class ListRowControl : UserControl
    {
        internal event Action<string?>? Selected;
        private double _h;
        internal ListRowControl(Models.Scenario scenario)
        {
            InitializeComponent();
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Stretch;
            grid.SizeChanged += (o, e) =>
            {
                _h = grid.Height = grid.ActualWidth / 20;
                province.FontSize = grid.ActualHeight / 2;
                scenarioCategory.FontSize = grid.ActualHeight / 2;
                scenarioName.FontSize = grid.ActualHeight / 2;
                description.FontSize = grid.ActualHeight / 2;
            };
            MouseLeftButtonDown += (o, e) => Selected?.Invoke(scenario.WebLink);
            scenarioName.Content = scenario.ScenarioName;
            scenarioCategory.Content = scenario.ScenarioCategory;
            province.Content = scenario.Province;
            description.Content = scenario.Description;
            //dpd.AddValueChanged(grid, (o, e) => {
            //    if (IsMouseOver)
            //        Enter(null, null);
            //    else
            //        Leave(null, null);
            //});
        }
        private readonly DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(Grid.IsMouseOverProperty, typeof(Grid));
        private async void Enter(object? sender, RoutedEventArgs? e)
        {
            for (int i = 0; i < 5; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    if (grid.IsMouseOver)
                    {
                        grid.Height = grid.ActualHeight + 3;
                        grid.Margin = new Thickness(i * 3, 0, 0, 0);
                        grid.Background = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {
                        Leave(null, null);
                    }
                });
            }
            await Task.Delay(40);
        }
        private void Leave(object? sender, RoutedEventArgs? e)
        {
            Dispatcher.Invoke(() =>
            {
                grid.Height = _h;
                grid.Margin = new Thickness(0, 0, 0, 0);
                grid.Background = new SolidColorBrush(Colors.Transparent);
            });
        }
    }
}
