using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace 屏幕管理
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var config = new ClientConfig()
            {
                //Code = "windowsTest",
                //AutoReboot = true,
                //DelayedUpdate = true,
                //GuardInterval = 100,
                //HeartBeatSecond = 50,
                //MaterialPath = "/",
                //MqttPassword = "public",
                //MqttPort = 333,
                //MqttServer = "123123123",
                //MqttUser = "admin",
                //ShowDownloader = true,
                //UpdateUrl = "localhos",
            };

        }
    }
}
