global using System;
global using System.Text.Json;
global using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VR文旅
{
    internal class Global
    {
        public const string PUBLIC_KEY = "";
        public const string HOST = "";
        private static readonly ViewWindow _view=new ViewWindow();
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        public static BitmapImage GetBitmap(string name)
        {
            using var stream = _assembly.GetManifestResourceStream(GetResourceName(name));
            BitmapImage bitmap = new();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }
        public static void ShowView(string url)
        {
            _view.Show(url);
        }
        public static void CloseView() => _view.Close();
        private static string GetResourceName(string name)
        {
            var result=string.Empty;
            foreach (var item in _assembly.GetManifestResourceNames())
                if(item.Contains(name))
                    result = item;
            return result;
        }
    }
}
