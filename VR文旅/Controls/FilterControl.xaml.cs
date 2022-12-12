using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// FilterControl.xaml 的交互逻辑
    /// </summary>
    public partial class FilterControl : UserControl
    {
        public FilterControl()
        {
            InitializeComponent();
            Margin = new Thickness(0,20,0,10);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
            Draw();
        }
        private void Draw()
        {
            grid.Children.Add(GetProvinceComboBox());
            grid.Children.Add(GetCityComboBox());
            grid.Children.Add(GetCategoryComboBox());
        }
        private static ComboBox GetProvinceComboBox()
        {
            ComboBox provinceComboBox = new()
            {
                Text = "选择省份",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            provinceComboBox.SetValue(Grid.RowProperty,0);
            provinceComboBox.SetValue(Grid.ColumnProperty, 0);
            provinceComboBox.Items.Add(new CheckBox { Content = "123" });
            provinceComboBox.Items.Add(new CheckBox { Content = "456" });
            provinceComboBox.Items.Add(new CheckBox { Content = "789" });
            return provinceComboBox;
        }
        private static ComboBox GetCityComboBox()
        {
            ComboBox cityComboBox = new()
            {
                Text = "选择城市",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment= VerticalAlignment.Center,
            };
            cityComboBox.SetValue(Grid.RowProperty, 0);
            cityComboBox.SetValue(Grid.ColumnProperty, 1);
            cityComboBox.Items.Add(new CheckBox { Content = "123" });
            cityComboBox.Items.Add(new CheckBox { Content = "456" });
            cityComboBox.Items.Add(new CheckBox { Content = "789" });
            return cityComboBox;
        }
        private static ComboBox GetCategoryComboBox()
        {
            ComboBox CategoryComboBox = new()
            {
                Text = "选择分类",
                Width = 80,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            CategoryComboBox.SetValue(Grid.RowProperty, 0);
            CategoryComboBox.SetValue(Grid.ColumnProperty, 2);
            CategoryComboBox.Items.Add(new CheckBox { Content = "123" });
            CategoryComboBox.Items.Add(new CheckBox { Content = "456" });
            CategoryComboBox.Items.Add(new CheckBox { Content = "789" });
            return CategoryComboBox;
        }
    }
}
