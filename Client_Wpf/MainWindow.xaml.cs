using SharedProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
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
        string url = "http://47.101.178.160:9009/videos/ebb403229cb089de04c40b7ddc81ba60CF17QQ.mp4";
        CustomControls.View view = new CustomControls.View();
        public MainWindow()
        {
            InitializeComponent();
            var client = new Client("ws://47.101.178.160:8083/mqtt", "max01");            
            var generate = new GenerateElement(MainGrid);
            client.LogWrite += (t, c, l) =>
            {
                //logView.AppendLog(t, c, (CustomControls.LogViewControl.LogLevelEnum)l);
            };
            client.Downloader += (m, c) =>
            {
                downloader.AddDownloadUri(m, c);
            };
            client.NormalContentEvent += content =>
            {
                view.SetNormalContents(content);
            };
            client.Emergency += content =>
            {
                view.SetEmergencyContent(content);
            };
            Closing += (o, e) => { view.Close(); };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var json = System.IO.File.ReadAllText("./Mock/MockContent.json");
            var normalcontent = NormalContent.FromJson(json);
            view.SetNormalContents(normalcontent);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            
            var downloadtask = new CustomControls.DownloadProgressControl.DownloadTask(0,url);
            downloader.AddDownloadUri(downloadtask, c => { MessageBox.Show($"{c.FileName}\n下载完成"); });
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            view.Visibility = Visibility.Visible;
        }
    }
}
