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

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// InformationControl.xaml 的交互逻辑
    /// </summary>
    public partial class InformationControl : UserControl
    {
        public InformationControl(string[] strings)
        {
            InitializeComponent();
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            foreach (var item in strings)
            {
                AddString(item);
            }
        }
        public void AddString(string content)
        {
            textBlock.Text += $"{content}\r";
        }
    }
}
