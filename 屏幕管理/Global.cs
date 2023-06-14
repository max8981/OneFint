global using System;
global using System.Linq;
global using System.IO;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Collections.Generic;
global using System.Threading;
using System.Collections.Concurrent;
using System.Xml.Linq;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Threading;

namespace 屏幕管理
{
    internal static class Global
    {
        private const long KB = 1024;
        private const long MB = 1024 * KB;
        private const long GB = 1024 * MB;
        private const long TB = 1024 * GB;
        private readonly static LayoutWindow _codeView = new(System.Windows.Forms.Screen.AllScreens.First());
        private readonly static Stopwatch _stopwatch = Stopwatch.StartNew();
        internal static Action<string, int> ShowMessage = (c, d) => { };
        internal static Action<string, string> MQTTLog = (t, c) => { };
        internal static Action<int, int?, int?> MaterialStatus = (c, d, g) => { };
        internal static Action<bool> Pause = x => { if (x) _stopwatch.Stop(); else _stopwatch.Start(); };
        internal static Action Jump = () => _stopwatch.Restart();
        internal static void ShowCode(string code) => _codeView.ShowCode(code);
        internal static void HiddenCode() => _codeView.HiddenCode();
        internal static DateTime Now => Systems.Time.Now;
        internal static string AppVersion => "1.0.0.4";
        internal static string IpAddress => Systems.Network.GetIPAddresses();
        internal static string MacAddress => Systems.Network.GetMacAddress();
        internal static int Rom => (int)(Systems.Memory.RootPathSize / 1000_000_000);
        internal static int Ram => (int)(Systems.Memory.PhisicalMemory / 1000_000_000);
        internal static int FreeDisk => (int)(Systems.Memory.RootPathSpace / 1000_000_000);
        internal static bool Activation { get; set; }
        internal static int? DeviceId { get; set; }
        internal static int? DeviceGroupId { get; set; }
        internal static bool IsDebug { get; set; }

        private static readonly ConcurrentDictionary<string, bool> _disengages = new();
        internal static System.Windows.Media.Imaging.BitmapImage GetImage(string imagePath)
        {
            System.Windows.Media.Imaging.BitmapImage bitmap = new();
            if (File.Exists(imagePath))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                using Stream ms = new MemoryStream(File.ReadAllBytes(imagePath));
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;
        }
        internal static void Disengage(this System.Windows.FrameworkElement element) => _disengages[element.Name] = true;
        internal static bool DelayHidden(this System.Windows.FrameworkElement element,string name, int second)
        {
            var ts = _stopwatch.Elapsed;
            SpinWait.SpinUntil(() => {
                var s = (_stopwatch.Elapsed - ts).TotalSeconds;
                return s <= 0 || second < s;
            });
            return true;
            _disengages.AddOrUpdate(name, false, (k, v) => v = false);
            return !SpinWait.SpinUntil(() => _disengages[name], second * 1000);
        }
        internal static bool DelayHidden(this System.Windows.FrameworkElement element,string name, int second, Func<bool> func)
        {
            _disengages.AddOrUpdate(name, false, (k, v) => v = false);
            return !SpinWait.SpinUntil(() => _disengages[name]|func(), second * 1000);
        }
        internal static System.Windows.Media.Brush ToBrush(this string? colorString)
        {
            var color = string.IsNullOrEmpty(colorString) ? System.Windows.Media.Colors.Transparent : 
                (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
            return new System.Windows.Media.SolidColorBrush(color);
        }
        internal static System.Windows.HorizontalAlignment ToHorizontal(this int horizontal) => horizontal switch
        {
            1 => System.Windows.HorizontalAlignment.Left,
            2 => System.Windows.HorizontalAlignment.Center,
            3 => System.Windows.HorizontalAlignment.Right,
            _ => System.Windows.HorizontalAlignment.Stretch,
        };
        internal static System.Windows.VerticalAlignment ToVertical(this int vertical) => vertical switch
        {
            1 => System.Windows.VerticalAlignment.Top,
            2 => System.Windows.VerticalAlignment.Center,
            3 => System.Windows.VerticalAlignment.Bottom,
            _ => System.Windows.VerticalAlignment.Stretch,
        };
        internal static System.Windows.TextAlignment ToTextAlignment(this int horizontal) => horizontal switch
        {
            1 => System.Windows.TextAlignment.Left,
            2 => System.Windows.TextAlignment.Center,
            3 => System.Windows.TextAlignment.Right,
            _ => System.Windows.TextAlignment.Justify,
        };
        internal static string ToBase64(this string filePath)
        {
            using FileStream filestream = new(filePath, FileMode.Open);
            byte[] bt = new byte[filestream.Length];
            filestream.Read(bt, 0, bt.Length);
            var base64Str = Convert.ToBase64String(bt);
            return base64Str;
        }
        internal static string ToByteString(this long value) => value switch
        {
            > TB => $"{value /= TB:0.00}TB",
            > GB => $"{value /= GB:0.00}GB",
            > MB => $"{value /= MB:0.00}MB",
            > KB => $"{value /= KB:0.00}KB",
            _ => $"{value}B",
        };
        internal static void Close()
        {
            foreach (var item in _disengages)
                _disengages[item.Key] = true;
        }
    }
}
