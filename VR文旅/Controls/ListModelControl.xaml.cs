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
        public event Action<string?>? Selected;
        public ListModelControl()
        {
            InitializeComponent();
            //dataGrid.MouseLeftButtonDown += (o, e) => ShowView();
            dataGrid.SelectionChanged += (o, e) =>
            {
                foreach (var item in e.AddedItems)
                {
                    if(item is Models.Scenario scenario)
                    {
                        Selected?.Invoke(scenario.WebLink);
                        return;
                    }
                }
            };
            Models.PLayLists.PlayListChanged += o =>
            {
                dataGrid.ItemsSource = Models.PLayLists.GetScenarios(0, 10);
            };
        }
        internal void Draw()
        {
            GetData(0);
        }
        private void pagination_PageChanged(object sender, RoutedEventArgs e)
        {
            if (e is System.Windows.RoutedPropertyChangedEventArgs<int> value)
                GetData(value.NewValue);
        }
        private void GetData(int page)
        {
            dataGrid.ItemsSource = Models.PLayLists.GetScenarios(page - 1, 10);
        }
    }
}
