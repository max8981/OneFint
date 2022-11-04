using ClientLibrary;
using ClientLibrary.UIs;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using AutoUpdaterDotNET;
using System.Text.Json.Serialization;
using System.Linq;
using System.Reflection;

namespace Client_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,ClientLibrary.IClient
    {
        const string VERSION = "1.0.0.5";
        private readonly ClientController _controller;
        private readonly List<string> _deleteFiles;
        public Action<DateTime> PowerOn => o => WindowsController.PowerOffAtDatePowerOn(o);
        public Action<DateTime> PowerOff { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<int> SetVolume => o => WindowsController.SetVolume(o);
        public Action<int> ScreenPowerOff => o => WindowsController.ScreenPowerOff();
        public Action<int> ScreenPowerOn => o => WindowsController.ScreenPowerOn();
        public Action<int> ScreenBrightness => o => WindowsController.SetScreenBrightness(o);
        public Action DeleteTempFiles => () =>
        {
            foreach (var item in System.IO.Directory.GetFiles(System.IO.Path.GetTempPath()))
            {
                System.IO.File.Delete(item);
            }
        };
        public Action<string, string> WriteLog => (t, c) => RichTextBoxWriteLog(t, c);
        public IPageController[] PageControllers { get; set; } 
        public ClientConfig Config { get; set; }
        public int Volume => WindowsController.GetVolume();
        public Action<string[]> DeleteFiles => o => _deleteFiles.AddRange(o);
        public MainWindow()
        {
            WindowsController.SetAutoBoot();
            InitializeComponent();
            App.ShowMessage = ShowMessage;
            ScreenModeListBox.ItemsSource = Enum.GetValues(typeof(DisplayController.ScreenModeEnum));
            var screen = System.Windows.Forms.Screen.AllScreens;
            _deleteFiles = new List<string>();
            WindowsController.PreventForCurrentThread();
            Config = new(this);
            if (System.IO.File.Exists("./Client_Wpf.dll.config"))
                Config.Load();
            if (Config.Code=="")
                Config.Code = Information.MachineCode;
            serverTextBox.Text = Config.MqttServer;
            usernameTextBox.Text = Config.MqttUser;
            passwordTextBox.Text = Config.MqttPassword;
            delayedUpdateCheckBox.IsChecked = Config.DelayedUpdate;
            showDownloaderCheckBox.IsChecked = Config.ShowDownloader;
            PageControllers = new PageView[System.Windows.Forms.Screen.AllScreens.Length];
            for (int i = 0; i < PageControllers.Length; i++)
            {
                PageControllers[i] = new PageView(System.Windows.Forms.Screen.AllScreens[i]);
                PageControllers[i].ShowCode(Config.Code);
                PageControllers[i].ShowView();
            }
            _controller = new(this);
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            hwndSource?.AddHook(new HwndSourceHook(WindowsController.WndProc));
        }
        private void RichTextBoxWriteLog(string title,string content)
        {
            Dispatcher.Invoke(() =>
            {
                if(flowDoc.Blocks.Count > 9)
                {
                    flowDoc.Blocks.Remove(flowDoc.Blocks.FirstBlock);
                    flowDoc.Blocks.Remove(flowDoc.Blocks.FirstBlock);
                }
                flowDoc.Blocks.Add(new Paragraph(new Bold(new Run($"[{DateTime.Now:G}]{title}"))));
                flowDoc.Blocks.Add(new Paragraph(new Run(content)));
                richTextBox.ScrollToEnd();
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WindowsController.PowerOffAtDatePowerOn(DateTime.Now.AddSeconds(30));
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (var page in PageControllers)
            {
                page.ShowView();
            }
        }
        internal void SetBrightnessValue(double d) => screenBrightness.Value = d;
        internal void SetVolumeValue(int vol) => systemVolume.Value = vol;
        private void ScreenBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //WindowsController.SetScreenBrightness(e.NewValue);
        }
        private void SystemVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WindowsController.SetVolume((int)e.NewValue);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var server= serverTextBox.Text.Trim();
            Config.MqttServer = server.Split(':')[0];
            if (server.Split(':').Length > 1)
            {
                if (int.TryParse(server.Split(':')[1], out var port))
                    Config.MqttPort = port;
            }
            Config.MqttUser = usernameTextBox.Text;
            Config.MqttPassword = passwordTextBox.Text;
            Config.Save();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _controller.Close();
            Config.Save();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (var item in _deleteFiles)
            {
                try
                {
                    System.IO.File.Delete(item);
                }
                catch(Exception ex)
                {
                    WriteLog("DeleteFiles", ex.Message);
                }

            }
            WindowsController.RestoreForCurrentThread();
            Process.GetCurrentProcess().Kill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.InstalledVersion = new Version(VERSION);
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.ParseUpdateInfoEvent += o =>
            {
                try
                {
                    var info = JsonSerializer.Deserialize<AutoUpdaterDotNET.UpdateInfoEventArgs>(o.RemoteData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    o.UpdateInfo = info;
                }
                catch
                {

                }

            };
            AutoUpdater.Start(Config.UpdateUrl);
        }

        private void ScreenModeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = e.AddedItems[0];
            if (s != null)
            {
                var o = s.ToString();
                DisplayController.SetScreenMode((DisplayController.ScreenModeEnum)Enum.Parse(typeof(DisplayController.ScreenModeEnum), o!));
            }
        }

        public void ShowMessage(string message, TimeSpan delay)
        {
            foreach (var page in PageControllers)
            {
                page.ShowMessage(message, delay);
            }
        }

        public void SaveConfiguration(string name, string value) => WindowsController.SaveConfiguration(name, value);

        public string LoadConfiguration(string name) => WindowsController.LoadConfiguration(name);
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            WindowsController.Reboot();
        }

        public void ShutDown()
        {
            WindowsController.ShutDown();
        }

        public void Reboot()
        {
            WindowsController.Reboot();
        }
        private void DelayedUpdateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var value = delayedUpdateCheckBox.IsChecked ?? true;
            Config.DelayedUpdate = value;
            ExhibitionController.SetDelayedUpdate(value);
            Config.Save();
        }

        private void ShowDownloaderCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var value = showDownloaderCheckBox.IsChecked ?? true;
            Config.DelayedUpdate = value;
            ExhibitionController.SetShowDownloader(value);
            Config.Save();
        }
    }
}
