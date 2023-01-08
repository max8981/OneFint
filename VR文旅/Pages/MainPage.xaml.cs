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

namespace VR文旅.Pages
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public event Action<string?>? Selected;
        public MainPage()
        {
            InitializeComponent();
            Getdata();
        }
        async void Getdata()
        {
            var data = await Models.PLayLists.GetPLayList(new Models.Location[] { new Models.Location("", "") }, new string[] { });
        }

        private void ListModelControl_Selected(string? obj)
        {
            Selected?.Invoke(obj);
        }

        private void ImageModelControl_Selected(string? obj)
        {
            Selected?.Invoke(obj);
        }
    }
}
