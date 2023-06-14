using CefSharp;
using CefSharp.DevTools.LayerTree;
using CefSharp.DevTools.Network;
using CefSharp.DevTools.Page;
using CefSharp.DevTools.WebAuthn;
using CefSharp.Handler;
using CefSharp.Lagacy;
using CefSharp.ResponseFilter;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Linq;
using VR文旅.Systems;

namespace VR文旅.Controls
{
    /// <summary>
    /// WebBrowserControl.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserControl : UserControl
    {
        private string _address = string.Empty;
        private static string[] _blocks = new string[]
            {
                "ads",
                "Ads",
                "topWp",
                "Logo",
                "RightBtnContainer",
                "TitleContainer",
                "Theme1_right",
                "Loading",
            };
        public event Action? Return;
        public WebBrowserControl()
        {
            InitializeComponent();
            var s = new CefSettings();
            s.SetOffScreenRenderingBestPerformanceArgs();
            //Cef.Initialize(s);
            //Cef.EnableHighDPISupport();
            image.Source = Global.GetBitmap("Arrow1");
            image.PreviewMouseLeftButtonDown += (o, e) =>
            {
                Visibility = Visibility.Hidden;
                Return?.Invoke();
            };
        }
        public async void Navigation(string? url)
        {
            if (string.IsNullOrEmpty(url)) return;
            var name = typeof(CefSharp.Wpf.ChromiumWebBrowser).Name;
            var chromium = await Dispatcher.Invoke(async () =>
            {
                if (grid.FindName(name) is not CefSharp.Wpf.ChromiumWebBrowser chromium)
                {
                    chromium = new();
                    chromium.FrameLoadEnd += (s, e) =>
                    {
                        chromium.SetZoomLevel(Config.ZoomLevel);
                    };
                    chromium.RequestHandler = new MyRequestHandler();
                    chromium.IsVisibleChanged += (o, e) => chromium.ToggleAudioMute();
                    chromium.HorizontalAlignment = HorizontalAlignment.Stretch;
                    chromium.VerticalAlignment = VerticalAlignment.Stretch;
                    grid.Children.Add(chromium);
                    grid.RegisterName(name, chromium);
                }
                chromium.Visibility = Visibility.Visible;
                _blocks = await Models.Block.GetBlocks();
                return chromium;
            });
            if (!_address.Equals(url))
                chromium.Address = url;
            _address = url;
        }
        public class MyRequestHandler : RequestHandler
        {
            protected override IResourceRequestHandler GetResourceRequestHandler(IWebBrowser webBrowser,IBrowser browser,IFrame frame,IRequest request,bool isNav,bool isDownload,string requestInitiator,ref bool disableDefaulHandling)
            {
                var url = request.Url;
                if (url.EndsWith(".css"))
                    return new MyResourceRequestHandler();
                return base.GetResourceRequestHandler(webBrowser, browser, frame, request, isNav, isDownload, requestInitiator, ref disableDefaulHandling);
            }
        }
        public class MyResourceRequestHandler : ResourceRequestHandler
        {
            protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
            {
                if (request.Url.Contains(".css"))
                    return new MyResourceHandler();
                return base.GetResourceHandler(chromiumWebBrowser, browser, frame, request);
            }
        }
        public class MyResourceHandler : CefSharp.Lagacy.ResourceHandler
        {
            private string _url = string.Empty;
            protected override bool ProcessRequestAsync(IRequest request, ICallback callback)
            {
                _url = request.Url;
                return base.ProcessRequestAsync(request, callback);
            }
            protected override Stream GetResponse(IResponse response, out long responseLength, out string redirectUrl)
            {
                var result = ReplaceCSS(_url);
                responseLength = result.Length;
                redirectUrl = null!;
                return result;
            }
        }
        private static Stream ReplaceCSS(string url)
        {
            var str = Http.GetAsync(url).Result;
            str = CSSHidden(str, _blocks);
            return new MemoryStream(Encoding.Default.GetBytes(str));
        }
        private static string CSSHidden(string str, string[] tags)
        {
            var sb = new StringBuilder(str);
            foreach (var item in tags)
            {
                var pattern = item + ".+?{";
                foreach (Match match in Regex.Matches(str, pattern).Cast<Match>())
                {
                    if (match.Success)
                    {
                        var key = match.Value;
                        var value = $"{key}display:none;";
                        sb = sb.Replace(key, value);
                    }
                }
            }
            return sb.ToString();
        }
    }
}
