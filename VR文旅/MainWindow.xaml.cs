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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VR文旅.Controls;

namespace VR文旅
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Pages.MainPage main;
        private readonly Pages.PlayPage play;
        public MainWindow()
        {
            InitializeComponent();
            //base.Width = SystemParameters.PrimaryScreenWidth;
            //base.Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;
            main = new();
            play = new();
            main.Selected += x => play.ShowWeb(x);
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
            this.Closing += (o, e) => play.Close();
            Loaded += (o, e) => frame.Navigate(main); 
        }
    }
}
