global using System;
global using System.Text.Json;
global using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace VR文旅
{
    internal class Global
    {
        public const string AppVersion = "1.0.0.5";
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetCursorPos(out MPoint point);
        private static Point _lastPoint = new();
        public const string PUBLIC_KEY = "";
        public const string HOST = "";
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        public static BitmapImage GetBitmap(string name)
        {
            BitmapImage bitmap = new();
            using var stream = _assembly.GetManifestResourceStream(GetResourceName(name));
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public static BitmapImage GetBitmap(FileInfo file)
        {
            System.Windows.Media.Imaging.BitmapImage bitmap = new();
            if (File.Exists(file.FullName))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                using Stream ms = new MemoryStream(File.ReadAllBytes(file.FullName));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }else
                bitmap=GetBitmap("Logo");
            return bitmap;
        }
        public static Image GetImage(string name)
        {
            Image result = new()
            {
                Source = GetBitmap(name)
            };
            return result;
        }
        public static Point GetMousePoint()
        {
            var result = new Point();
            if (GetCursorPos(out MPoint point))
                result = new(point.X, point.Y);
            return result;
        }
        public static bool IsMouseMove()
        {
            var point = GetMousePoint();
            bool result;
            if (result = !point.Equals(_lastPoint))
                _lastPoint = point;
            return result;
        }
        private static string GetResourceName(string name)
        {
            var result=string.Empty;
            foreach (var item in _assembly.GetManifestResourceNames())
                if(item.Contains(name))
                    result = item;
            return result;
        }
        internal static System.Windows.Media.Imaging.BitmapImage GetUrlImage(string url)
        {
            System.Windows.Media.Imaging.BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(url);
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MPoint
        {
            public int X;
            public int Y;
            public MPoint(int x, int y)
            {
                X=x; Y=y;
            }
        }
    }
}
