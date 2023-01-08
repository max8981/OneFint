using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
    /// ImageModelView.xaml 的交互逻辑
    /// </summary>
    public partial class ImageModelView : UserControl
    {
        public event Action<string?>? Selected;
        private static Thickness _thickness = new(10, 10, 10, 10);
        private readonly string? url;
        internal ImageModelView(Models.Scenario scenario)
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) => grid.Width = grid.ActualHeight / 3;
            url = scenario.WebLink;
            Draw(scenario);
        }
        private void Draw(Models.Scenario scenario)
        {
            grid.Children.Add(GetImage(scenario.ThumbUrl));
            grid.Children.Add(GetLabel($"{scenario.City} {scenario.ScenarioName}"));
            grid.Children.Add(GetTextBlock(scenario.Description));
        }
        private void ShowView()
        {
            Selected?.Invoke(url);
        }
        private Image GetImage(string? source)
        {
            Image image = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin= _thickness,
            };
            if (!string.IsNullOrWhiteSpace(source))
                image.Source = new BitmapImage(new Uri(source));
            image.MouseLeftButtonDown += (o, e) => ShowView();
            Grid.SetRow(image, 0);
            return image;
        }
        private Label GetLabel(string source)
        {
            Label label = new()
            {
                Content = source,
                Margin= _thickness,
            };
            label.MouseLeftButtonDown += (o, e) => ShowView();
            Grid.SetRow(label, 1);
            return label;
        }
        private TextBlock GetTextBlock(string? source)
        {
            TextBlock textBlock = new()
            {
                Text = source,
                TextWrapping = TextWrapping.Wrap,
                Margin= _thickness,
            };
            textBlock.MouseLeftButtonDown += (o, e) => ShowView();
            Grid.SetRow(textBlock, 2);
            return textBlock;
        }
    }
}
