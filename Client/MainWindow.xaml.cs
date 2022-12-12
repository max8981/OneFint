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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ClientController _controller;
        private readonly ClientConfig _config;
        private readonly Dictionary<int, LayoutWindow> _layouts = new();
        public MainWindow()
        {
            InitializeComponent();
            _controller = new ClientController();
            _config = new ClientConfig();
            _layouts.Add(0, new LayoutWindow());
            var client = new ClientCore.Controllers.ClientController(_controller, _config, _layouts.Values.ToArray());
            client.StartAll();
        }
    }
}
