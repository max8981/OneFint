using CefSharp.DevTools.CSS;
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
        private double _lastWidth = 0;
        public FilterControl()
        {
            InitializeComponent();
            grid.SizeChanged += (o, e) =>
            {
                if (_lastWidth != grid.ActualWidth)
                {
                    province.FontSize = grid.ActualHeight / 2;
                    province.Width = grid.ActualHeight * 3;
                    city.FontSize = grid.ActualHeight / 2;
                    city.Width = grid.ActualHeight * 3;
                    type.FontSize = grid.ActualHeight / 2;
                    type.Width = grid.ActualHeight * 3;
                    _lastWidth = grid.ActualWidth;
                }
            };
            Loaded += async (o, e) =>
            {
                await Models.ProvinceAndCity.GetProvinceAndCityAsync();
                await Models.ScenarioCategories.GetScenarioCategoriesAsync();
            };
            Margin = new Thickness(0,20,0,10);
            HorizontalAlignment = HorizontalAlignment.Stretch;
            VerticalAlignment = VerticalAlignment.Top;
            province.DropDownOpened += ProvinceDropDownOpened;
            province.DropDownClosed += DropDownClosed;
            province.SelectionChanged += SelectionChanged;
            city.DropDownOpened += CityDropDownOpened;
            city.DropDownClosed += DropDownClosed;
            city.SelectionChanged += SelectionChanged;
            type.DropDownOpened += TypeDropDownOpened;
            type.DropDownClosed += DropDownClosed;
            type.SelectionChanged += TypeSelectionChanged;
        }
        private static void ProvinceDropDownOpened(object? sender, EventArgs e)
        {
            var province = sender as ComboBox;
            if (province is not null)
            {
                province.Items.Clear();
                foreach (var location in Models.ProvinceAndCity.GetProvinces())
                    province.Items.Add(GetLocationCheckBox(location.Key.Province, location.Key, location.Value));
            }
        }
        private static void ProvinceDropDownClosed(object? sender, EventArgs e)
        {
            var province = sender as ComboBox;
            if (province is not null)
            {
                //foreach (var item in province.Items)
                //{
                //    if (item is CheckBox checkBox)
                //        if (checkBox.IsChecked.GetValueOrDefault(false))
                //            _selected.Add(checkBox.Content.ToString() ?? "", new List<string>());
                //}
            }
        }
        private static void CityDropDownOpened(object? sender, EventArgs e)
        {
            var city = sender as ComboBox;
            if(city is not null)
            {
                city.Items.Clear();
                var locations = Models.ProvinceAndCity.GetCitys();
                foreach (var location in locations)
                    city.Items.Add(GetLocationCheckBox(location.Key.City, location.Key, location.Value));
            }
        }
        private static void CityDropDownClosed(object? sender, EventArgs e)
        {
            var city = sender as ComboBox;
            if (city is not null)
            {

            }
        }
        private void TypeDropDownOpened(object? sender, EventArgs e)
        {
            var type = sender as ComboBox;
            if (type is not null)
            {
                type.Items.Clear();
                foreach (var category in Models.ScenarioCategories.Types)
                    type.Items.Add(GetTypeCheckBox(category));
            }
        }
        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
                if (item is CheckBox checkBox)
                {
                    var name = checkBox.Name;
                    Models.ScenarioCategories.SetValue(name, !Models.ScenarioCategories.GetValue(name));
                }
        }
        private static void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
                if (item is CheckBox checkBox)
                {
                    var location = (Models.Location)checkBox.Tag;
                    Models.ProvinceAndCity.SetValue(location, !Models.ProvinceAndCity.Location[location]);
                }
        }
        private static async void DropDownClosed(object? sender, EventArgs e)
        {
            await Models.PLayLists.GetPLayList(Models.ProvinceAndCity.GetLocations(),Models.ScenarioCategories.GetTypes());
        }
        private static CheckBox GetTypeCheckBox(string text)
        {
            var checkbox = new CheckBox
            {
                Name = text,
                Content = text
            };
            return checkbox;
        }
        private static CheckBox GetLocationCheckBox(string content, Models.Location location,bool check)
        {
            var checkbox = new CheckBox
            {
                Tag = location,
                Content = content,
                IsChecked = check,
            };
            checkbox.Click += (o, e) => Models.ProvinceAndCity.SetValue(location, !Models.ProvinceAndCity.Location[location]);
            return checkbox;
        }
    }
}
