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

namespace Client_Wpf
{
    /// <summary>
    /// DownloadControl.xaml 的交互逻辑
    /// </summary>
    public partial class DownloadControl : UserControl
    {
        public DownloadControl()
        {
            InitializeComponent();
            titleLabel.Foreground = new SolidColorBrush(Colors.White);
            statusLabel.Foreground = new SolidColorBrush(Colors.White);
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            progress.Maximum = 100;
            progress.Value = 0;
        }
        public void SetProgress(string title,string content,float value)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                titleLabel.Content = title;
                statusLabel.Content = content;
                progress.Value = value*100;
            }));
        }
    }
}
