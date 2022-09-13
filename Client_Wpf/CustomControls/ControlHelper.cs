using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client_Wpf.CustomControls
{
    public static class ControlHelper
    {
        public static Brush GetBrush(string colorstring)
        {
            if (string.IsNullOrWhiteSpace(colorstring))
            {
                return null;
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorstring));
            }
        }
        public static System.Windows.HorizontalAlignment GetHorizontal(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.HorizontalAlignment.Left,
                2 => System.Windows.HorizontalAlignment.Center,
                3 => System.Windows.HorizontalAlignment.Right,
                _ => System.Windows.HorizontalAlignment.Stretch,
            };
        }
        public static System.Windows.VerticalAlignment GetVertical(int vertical)
        {
            return vertical switch
            {
                1 => System.Windows.VerticalAlignment.Top,
                2 => System.Windows.VerticalAlignment.Center,
                3 => System.Windows.VerticalAlignment.Bottom,
                _ => System.Windows.VerticalAlignment.Stretch,
            };
        }
        public static System.Windows.TextAlignment GetTextAlignment(int horizontal)
        {
            return horizontal switch
            {
                1 => System.Windows.TextAlignment.Left,
                2 => System.Windows.TextAlignment.Center,
                3 => System.Windows.TextAlignment.Right,
                _ => System.Windows.TextAlignment.Justify,
            };
        }
    }
}
