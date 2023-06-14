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
        private bool _listMode;
        public event Action<string?>? Selected;
        public event Action? Exit;
        public MainPage()
        {
            InitializeComponent();
            SizeChanged += (o, e) =>
            {
                groupSelect.Height = ActualHeight / 10;
            };
            groupSelect.Source = Global.GetBitmap("Group5");
            groupSelect.MouseLeftButtonDown += (o, e) =>
            {
                _listMode = !_listMode;
                if (_listMode)
                    ListModel();
                else
                    ImageModel();
            };
            image.Selected += ImageModelControl_Selected;
            list.Selected += ListModelControl_Selected;
            exitButton.PreviewMouseLeftButtonDown += (o, e) =>
            {
                var messageResult = MessageBox.Show("退出","确认退出VR文旅？",MessageBoxButton.YesNo);
                if (messageResult == MessageBoxResult.Yes)
                    Exit?.Invoke();
            };
            exitButton.Source= Global.GetBitmap("Exit");
            Getdata();
        }
        private void ImageModel()
        {
            groupSelect.Source = Global.GetBitmap("Group5");
            tabControl.SelectedIndex = 0;
        }
        private void ListModel()
        {
            groupSelect.Source = Global.GetBitmap("Group6");
            tabControl.SelectedIndex= 1;
        }
        public void PrevPage()
        {
            if (!_listMode)
                image.PrevPage();
        }
        public void NextPage()
        {
            if (!_listMode)
                image.NextPage();
        }
        public void ShowExitButton(bool visibility)
        {
            if (visibility)
                exitButton.Visibility = Visibility.Visible;
            else
                exitButton.Visibility = Visibility.Hidden;
        }

        static async void Getdata()
        {
            await Models.PLayLists.GetPLayList(new Models.Location[] { new Models.Location("", "") }, "");
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
