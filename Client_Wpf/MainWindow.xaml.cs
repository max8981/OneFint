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

namespace Client_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,ClientLibrary.IClient
    {
        private readonly ClientController _controller;
        private readonly List<string> _deleteFiles;

        public Action<DateTime> PowerOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<DateTime> PowerOff { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action Reboot { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action<int> SetVolume => o => WindowsController.SetVolume(o);
        public Action<int> ScreenPowerOff => o => WindowsController.ScreenPowerOff();
        public Action<int> ScreenPowerOn => o => WindowsController.ScreenPowerOn();
        public Action<int> ScreenBrightness => o => WindowsController.SetScreenBrightness(o);
        public Action DeleteTempFiles => () =>
        {
            //System.IO.Directory.Delete(System.IO.Path.GetTempPath());
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
            InitializeComponent();
            _deleteFiles = new List<string>();
            WindowsController.PreventForCurrentThread();
            Config = WindowsController.LoadConfiguration<ClientConfig>(nameof(Config));
            Config.Code = Information.MachineCode;
            serverTextBox.Text = Config.Server;
            usernameTextBox.Text = Config.MqttUser;
            passwordTextBox.Text = Config.MqttPassword;
            delayedUpdateCheckBox.IsChecked = Config.DelayedUpdate;
            showDownloaderCheckBox.IsChecked = Config.ShowDownloader;
            PageControllers = new PageView[] { new PageView() };
            foreach (var pageController in PageControllers)
            {
                pageController.ShowCode(Config.Code);
                pageController.ShowView();
            }
            _controller = new(this);
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
        private void screenBrightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WindowsController.SetScreenBrightness(e.NewValue);
        }
        private void systemVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WindowsController.SetVolume((int)e.NewValue);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Config.Server = serverTextBox.Text.Trim();
            Config.MqttUser = usernameTextBox.Text;
            Config.MqttPassword = passwordTextBox.Text;
            Save(nameof(Config), Config);
        }

        public void Save<T>(string key, T value) => WindowsController.SaveConfiguration(key, value);

        public T Load<T>(string key) where T : new() => WindowsController.LoadConfiguration<T>(key);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _controller.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox check)
                if (check.IsChecked.HasValue)
                {
                    if(Config.DelayedUpdate != check.IsChecked.Value)
                    {
                        Config.DelayedUpdate = check.IsChecked.Value;
                        _controller.SetDelayedUpdate(Config.DelayedUpdate);
                        Save(nameof(Config), Config);
                    }
                }
        }

        private void showDownloaderCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox check)
                if (check.IsChecked.HasValue)
                {
                    if (Config.ShowDownloader != check.IsChecked.Value)
                    {
                        Config.ShowDownloader = check.IsChecked.Value;
                        _controller.SetShowDownloader(Config.ShowDownloader);
                        Save(nameof(Config), Config);
                    }
                }
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
            var a = Process.GetCurrentProcess();
            WindowsController.RestoreForCurrentThread();
            Process.GetCurrentProcess().Kill();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AutoUpdater.InstalledVersion = new Version("1.1");
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.ForcedDownload;
            AutoUpdater.ParseUpdateInfoEvent += o =>
            {
                var info = JsonSerializer.Deserialize<AutoUpdaterDotNET.UpdateInfoEventArgs>(o.RemoteData,new JsonSerializerOptions {PropertyNameCaseInsensitive=true });
                o.UpdateInfo = info;
            };
            AutoUpdater.Start("http://localhost:5000/api/update");
        }
    }
}
