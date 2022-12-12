using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.CustomControls
{
    internal static class ControlEx
    {
        private static readonly Dictionary<string, bool> _disengages = new();
        public static BitmapImage GetImage(string imagePath)
        {
            BitmapImage bitmap = new();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                using Stream ms = new MemoryStream(File.ReadAllBytes(imagePath));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;
        }
        public static void Disengage(this FrameworkElement element) => _disengages[element.Name] = true;
        public static bool DelayHidden(this FrameworkElement element, int second)
        {
            _disengages[element.Name] = false;
            element.Visibility = Visibility.Visible;
            var result = !SpinWait.SpinUntil(() => _disengages[element.Name], second * 1000);
            element.Visibility = Visibility.Hidden;
            return result;
        }
        public static bool DelayHidden(this FrameworkElement element, int second, Func<bool> func)
        {
            _disengages[element.Name] = false;
            element.Visibility = Visibility.Visible;
            var result = !SpinWait.SpinUntil(() => _disengages[element.Name] | func(), second * 1000);
            element.Visibility = Visibility.Hidden;
            return result;
        }
        public static Brush ToBrush(this string? colorString)
        {
            var color = string.IsNullOrEmpty(colorString) ? Colors.Transparent : (Color)ColorConverter.ConvertFromString(colorString);
            return new SolidColorBrush(color);
        }
        public static System.Windows.HorizontalAlignment ToHorizontal(this int horizontal) => horizontal switch
        {
            1 => System.Windows.HorizontalAlignment.Left,
            2 => System.Windows.HorizontalAlignment.Center,
            3 => System.Windows.HorizontalAlignment.Right,
            _ => System.Windows.HorizontalAlignment.Stretch,
        };
        public static System.Windows.VerticalAlignment ToVertical(this int vertical)=> vertical switch
        {
            1 => System.Windows.VerticalAlignment.Top,
            2 => System.Windows.VerticalAlignment.Center,
            3 => System.Windows.VerticalAlignment.Bottom,
            _ => System.Windows.VerticalAlignment.Stretch,
        };
        public static System.Windows.TextAlignment ToTextAlignment(this int horizontal)=> horizontal switch
        {
            1 => System.Windows.TextAlignment.Left,
            2 => System.Windows.TextAlignment.Center,
            3 => System.Windows.TextAlignment.Right,
            _ => System.Windows.TextAlignment.Justify,
        };
    }
}
