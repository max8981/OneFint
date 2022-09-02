using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Client_Wpf
{
    internal class GenerateElement
    {
        private readonly Grid _grid;
        private static DispatcherTimer _dispatcher = new DispatcherTimer();
        public GenerateElement(Grid grid)
        {
            _grid = grid;
        }
        private static Brush GetBrush(string colorString)
        {
            var color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(colorString);
            return new SolidColorBrush(color);
        }
        private static System.Windows.HorizontalAlignment GetHorizontal(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.HorizontalAlignment.Left,
                2 => System.Windows.HorizontalAlignment.Center,
                3 => System.Windows.HorizontalAlignment.Right,
                _ => System.Windows.HorizontalAlignment.Left,
            };
        }
        private static System.Windows.VerticalAlignment GetVertical(int vertical)
        {
            return vertical switch
            {
                1 => System.Windows.VerticalAlignment.Top,
                2 => System.Windows.VerticalAlignment.Center,
                3 => System.Windows.VerticalAlignment.Bottom,
                _ => System.Windows.VerticalAlignment.Top,
            };
        }
    }
}