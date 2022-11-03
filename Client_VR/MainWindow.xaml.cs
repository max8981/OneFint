using Client_VR.CustomControls;
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

namespace Client_VR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewControl _viewControl = new() { Visibility=Visibility.Hidden};
        public MainWindow()
        {
            InitializeComponent();
            grid.Children.Add(_viewControl);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewControl.Visibility = Visibility.Visible;
        }
    }
}
