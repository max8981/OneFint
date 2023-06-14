using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using 屏幕管理.Systems;

namespace 屏幕管理
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Controllers.ClientController _controller;
        public MainWindow()
        {
            InitializeComponent();
            Config.Load();
            Title += $"    Code:{Config.Code}    ver:{Global.AppVersion}";
            if (Config.Dns != null)
                Systems.Network.SetDns(Config.Dns);
            TimeSpan? ts = Config.FakeShutdown ? null : Config.AutoReboot ? new TimeSpan(Config.RebootTimeHours, Global.Now.Minute, 0) : null;
#if DEBUG
            Global.IsDebug = true;
#endif
            if (!Global.IsDebug)
            {
                Regedit.SetAutoRun();
                if (Regedit.DisableUAC())
                    if (MessageBox.Show("设置UAC成功，需要重启计算机", "立即重启？", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Systems.Power.Reboot();
                Guard.StartWatchdog(System.Windows.Forms.Application.ExecutablePath, Config.GuardInterval, ts);
            }
            serverText.Text = Config.MqttServer;
            portText.Text = Config.MqttPort.ToString();
            updateUrlText.Text = Config.UpdateUrl;
            Controllers.AutoUpdateController.NetUpdate(Config.UpdateUrl);
            Global.MQTTLog = (t, c) => RichTextBoxWriteLog(t, c);
            _controller = new(System.Windows.Forms.Screen.AllScreens);
            Closing += (o, e) =>
            {
                Config.Save();
                Global.Close();
                _controller.Close();
                Log.Default.Close();
            };
            Closed += (o, e) =>
            {
                Guard.RestoreForCurrentThread();
                Process.GetCurrentProcess().Kill();
            };
            _controller.Connect();
            this.WindowState = WindowState.Minimized;
        }
        private void RichTextBoxWriteLog(string title, string content)
        {
            Dispatcher.Invoke(() =>
            {
                if (flowDoc.Blocks.Count > 9)
                {
                    flowDoc.Blocks.Remove(flowDoc.Blocks.FirstBlock);
                    flowDoc.Blocks.Remove(flowDoc.Blocks.FirstBlock);
                }
                flowDoc.Blocks.Add(new Paragraph(new Bold(new Run($"[{DateTime.Now:G}]{title}"))));
                flowDoc.Blocks.Add(new Paragraph(new Run(content)));
                richTextBox.ScrollToEnd();
            });
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            hwndSource?.AddHook(new HwndSourceHook(Controllers.AutoUpdateController.WndProc));
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var uri = new Uri(updateUrlText.Text);
                Controllers.AutoUpdateController.CheckUpdate(uri.ToString());
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "UpdateButton_Click", $"Parameter:{updateUrlText.Text}");
                MessageBox.Show("无效的Url");
            }
        }
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var server=serverText.Text;
            if(int.TryParse(portText.Text,out var port))
                if (await _controller.ReConnect(server, port))
                {

                }
            Config.MqttServer = server;
            Config.MqttPort = port;
            Config.Save();
        }
        private void ShowLayoutButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var layout in _controller.Layouts)
                layout.ShowView();
        }

        private void CloseBotton_Click(object sender, RoutedEventArgs e)
        {
            Guard.StopWatchDog();
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.U)
                if (MessageBox.Show("是否卸载？", "卸载", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Systems.Regedit.DeleteAutoRun();
                    if (Systems.Regedit.EnableUAC())
                        if (MessageBox.Show("恢复UAC成功，需要重启计算机", "立即重启", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            Systems.Power.Reboot();
                    Guard.StopWatchDog();
                    Close();
                }
        }
    }
}
