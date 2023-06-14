using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// PolygonControl.xaml 的交互逻辑
    /// </summary>
    public partial class PolygonControl : UserControl
    {
        private const double RATIO = 3;
        public PolygonControl(string content)
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) => Draw(content);
            label.Content = content;
            image.Source = Global.GetBitmap("Polygon4");
            Draw(content);
        }
        private void Draw(string content)
        {
            if (Height > 0)
            {
                grid.Width = Height * RATIO;
                grid.Height = Height;
            }
            else if (Width > 0)
            {
                grid.Height = Width / RATIO;
                grid.Width = Width;
            }
            var size = SetFontSize(content, FontSize);
            grid.Height = size.Height + 10;
            grid.Width = size.Height + size.Width + 10;
            imageCol.Width = new GridLength(size.Height, GridUnitType.Pixel);
        }
        private Size SetFontSize(string content, double fontSize)
        {
            var size = MeasureTextWidth(content, fontSize, label.FontFamily.ToString());
            if (size.Height < grid.ActualHeight * 0.6)
                return SetFontSize(content, fontSize * 1.1);
            label.FontSize = fontSize;
            return size;
        }
        private Size MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new(text,CultureInfo.InvariantCulture,
    FlowDirection.LeftToRight, new Typeface(fontFamily.ToString()),
    fontSize,
    Brushes.Black,
    VisualTreeHelper.GetDpi(this).PixelsPerDip);
            return new(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }
    }
}
