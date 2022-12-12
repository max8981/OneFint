using System;
using System.Collections.Generic;
using System.Data;
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

namespace VR文旅.Controls
{
    /// <summary>
    /// ListModelControl.xaml 的交互逻辑
    /// </summary>
    public partial class ListModelControl : UserControl
    {
        public ListModelControl()
        {
            InitializeComponent();
            //dataGrid.MouseLeftButtonDown += (o, e) => ShowView();
            dataGrid.SelectionChanged+= (o, e) => ShowView();
            GetData(1);
        }
        private void Draw(IEnumerable<Type> data)
        {
            dataGrid.ItemsSource = data;
        }
        private void pagination_PageChanged(object sender, RoutedEventArgs e)
        {
            if (e is System.Windows.RoutedPropertyChangedEventArgs<int> value)
                GetData(value.NewValue);
        }
        private void GetData(int page)
        {
            dataGrid.ItemsSource = Models.Fake.GetPlayList(10,page).Scenarios;
        }
        private void ShowView()
        {
            Global.ShowView("https://www.720yun.com/t/83cjegevuw2?scene_id=17199073");
        }
    }
}
