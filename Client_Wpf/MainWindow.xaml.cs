using Client_Wpf.CustomControls;
using ClientLibrary;
using CoreAudio;
using Microsoft.Win32.SafeHandles;
using SharedProject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, uint wParam, int iParam);
        [DllImport("kernel32.dll")]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset,string lpTimerName);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutin, bool fResume);
        private const uint WM_SYSCOMMAND = 0x0112;
        private const uint SC_MONITORPOWER = 0xF170;
        private readonly IntPtr _handle = new(0xffff);
        private readonly IntPtr _currentMonitor;
        private readonly DisplayController.PHYSICAL_MONITOR[] _MONITORs;
        private readonly MMDevice _playbackDevice;
        CustomControls.View view = new CustomControls.View();
        readonly ClientConnect _clientConnect = new(new ClientLibrary.ClientConfig());
        public MainWindow()
        {
            InitializeComponent();
            _playbackDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            _currentMonitor = DisplayController.GetCurrentMonitor();
            _MONITORs= DisplayController.GetPhysicalMonitors(_currentMonitor);
            var ge = new GenerateElement(view);
            ClientLibrary.ClientController.WriteLog = WiterLog;
            ClientController.NormalContent = ge.NormalContent;
            ClientController.ScreenPowerOff = o => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, 2);
            ClientController.ScreenPowerOn = o => SendMessage(_handle, WM_SYSCOMMAND, SC_MONITORPOWER, -1);
            ClientController.SetVolume = o =>
            {
                if (_playbackDevice.AudioEndpointVolume != null)
                {
                    o = o > 100 ? 100 : o;
                    o = o < 0 ? 0 : o;
                    _playbackDevice.AudioEndpointVolume.MasterVolumeLevelScalar = o / 100f;
                }
            };
            ClientController.ScreenBrightness = o =>
            {
                o = o > 100 ? 100 : o;
                o = o < 0 ? 0 : o;
                foreach (var monitor in _MONITORs)
                {
                    DisplayController.SetMonitorBrightness(monitor, o/100f);
                }
            };
            var info = new ClientLibrary.Information();
            var infoview = new CustomControls.InformationControl(new string[] { info.DiskSize });
            view.AddControl("info", infoview);
            _clientConnect.Conntect();
            Closing += (o, e) => { view.Close(); };


        }
        private void WiterLog(string title,string content)
        {
            Dispatcher.Invoke(() =>
            {
                flowDoc.Blocks.Add(new Paragraph(new Bold(new Run(title))));
                flowDoc.Blocks.Add(new Paragraph(new Run(content)));
            });
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long duetime = DateTime.Now.AddMinutes(1).ToFileTime();
            using SafeWaitHandle handle = CreateWaitableTimer(IntPtr.Zero, true, "MyWaitabletimer");
            if(SetWaitableTimer(handle,ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
            {
                using EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset);
                wh.SafeWaitHandle = handle;
                Process.Start("shutdown.exe", "-h");
                wh.WaitOne();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            view.Visibility = Visibility.Visible;
        }
    }
}
