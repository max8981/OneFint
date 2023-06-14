using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VR文旅.Controls;
using VR文旅.Systems;

namespace VR文旅
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Pages.MainPage main;
        private readonly Pages.PlayPage play;

        private Point _startPoint;
        private bool _isMouseRightMove;
        private bool _isMouseLeftMove;
        private bool _isMouseTopMove;
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            base.Width = SystemParameters.PrimaryScreenWidth;
            base.Height = SystemParameters.PrimaryScreenHeight;
            ResizeMode = ResizeMode.NoResize;
            Left = 0;
            Top = 0;
            main = new();
            play = new();
            main.Selected += x => play.ShowWeb(x);
            main.Exit += () => Close();
            play.Show += x =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (x)
                        frame.Navigate(play);
                    else
                        frame.Navigate(main);
                });
            };
            this.Closing += (o, e) => { play.Close(); Systems.Config.Save(); };
            Loaded += (o, e) => { 
                Systems.Config.Load(); 
                frame.Navigate(main);
                if (string.IsNullOrWhiteSpace(Systems.Config.MacAddress))
                    Systems.Config.MacAddress = Network.GetMacAddress().Replace("-", "");
            };
            PreviewMouseLeftButtonDown += (o, e) =>
            {
                _startPoint = e.MouseDevice.GetPosition(this);
            };
            PreviewMouseLeftButtonUp += (o, e) =>
            {
                if (_isMouseLeftMove)
                    main.NextPage();
                if (_isMouseRightMove)
                    main.PrevPage();
                if (_isMouseTopMove)
                    this.WindowState = WindowState.Minimized;
            };
            PreviewMouseMove += (o, e) =>
            {
                if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    _isMouseRightMove = false;
                    _isMouseLeftMove = false;
                    _isMouseTopMove = false;
                    var point = e.MouseDevice.GetPosition(this);
                    var x = point.X - _startPoint.X;
                    var y = _startPoint.Y - point.Y;
                    if (ActualHeight - _startPoint.Y < 40)
                    {
                        if (y > 100)
                            _isMouseTopMove = true;
                    }
                    else if (Math.Abs(x) > 50)
                    {
                        if (x > 0)
                            _isMouseRightMove = true;
                        else
                            _isMouseLeftMove = true;
                    }
                }
                else
                {
                    var point = e.MouseDevice.GetPosition(this);
                    var x = ActualWidth - point.X;
                    main.ShowExitButton(x < 48 && point.Y < 48);
                }
            };
            AutoUpdateController.NetUpdate(Config.UpdateUrl);
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            Log.IsHtmLog = true;
            AutoUpdater.InstalledVersion = new Version(Global.AppVersion);
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            hwndSource?.AddHook(new HwndSourceHook(AutoUpdateController.WndProc));
        }
    }
}
