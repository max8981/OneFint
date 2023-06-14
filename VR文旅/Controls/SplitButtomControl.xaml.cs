using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace VR文旅.Controls
{
    /// <summary>
    /// SplitButtomControl.xaml 的交互逻辑
    /// </summary>
    public partial class SplitButtomControl : UserControl
    {
        private readonly Dictionary<string, bool> _data;
        public event Action<string[]>? Selected;
        public bool IsSingleSelection { get; set; }
        public SplitButtomControl()
        {
            InitializeComponent();
            _data = new();
            grid.SizeChanged += (o, e) => Draw();
            image.Source = Global.GetBitmap("Down");
            Draw();
            grid.MouseLeftButtonDown += (o, e) =>
            {
                comboBox.IsDropDownOpen = !comboBox.IsDropDownOpen;
                if(comboBox.IsDropDownOpen )
                {
                    border.BorderBrush = new SolidColorBrush(Colors.Gray);
                    border.BorderThickness = new(2, 2, 0, 0);
                }
            };
            comboBox.DropDownOpened += (o, e) => AddItems();
            comboBox.DropDownClosed += (o, e) => Selected?.Invoke(GetSelected());
            comboBox.SelectionChanged += SelectionChanged;
        }
        public string Title { get => label.Content.ToString() ?? "";set=> label.Content = value; }
        public void SetData(string[] strings)
        {
            _data.Clear();
            foreach (string s in strings)
                _data.Add(s, false);
        }
        public string[] GetSelected()
        {
            return _data.Where(x=>x.Value).Select(x=>x.Key).ToArray();
        }
        public string GetSingleSelected()
        {
            return _data.Where(x => x.Value).Select(x => x.Key).ToArray().FirstOrDefault() ?? "";
        }
        private void Draw()
        {
            var content = Title;
            border.CornerRadius = new CornerRadius(grid.ActualHeight / 2);
            borderGrid.Margin = new(grid.ActualHeight / 2, 0, grid.ActualHeight / 2, 0);
            var size = SetFontSize(content, label.FontSize);
            imageCol.Width = new GridLength(size.Height, GridUnitType.Pixel);
            comboBox.FontSize= label.FontSize;
        }
        private void AddItems()
        {
            comboBox.Items.Clear();
            foreach (var item in _data)
                comboBox.Items.Add(GetCheckBox(item.Key, item.Value));
        }
        private CheckBox GetCheckBox(string text, bool check)
        {
            var checkbox = new CheckBox
            {
                Name = text,
                Content = text,
                IsChecked = check,
            };
            checkbox.Click += (o, e) =>
            {
                if (IsSingleSelection&& !_data[text])
                    foreach (var item in _data)
                        _data[item.Key] = false;
                _data[text] = !_data[text];
            };
            return checkbox;
        }
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
                if (item is CheckBox checkBox)
                {
                    if (IsSingleSelection && !_data[checkBox.Name])
                        foreach (var s in _data)
                            _data[s.Key] = false;
                    _data[checkBox.Name] = !_data[checkBox.Name];
                }
        }
        private Size SetFontSize(string content,double fontSize) 
        {
            var size = MeasureTextWidth(content, fontSize, label.FontFamily.ToString());
            if (size.Height < grid.ActualHeight*0.6)
                return SetFontSize(content, fontSize * 1.1);
            label.FontSize = fontSize;
            return size;
        }
        private Size MeasureTextWidth(string text, double fontSize, string fontFamily)
        {
            FormattedText formattedText = new(text, CultureInfo.InvariantCulture,
    FlowDirection.LeftToRight, new Typeface(fontFamily.ToString()),
    fontSize,
    Brushes.Black,
    VisualTreeHelper.GetDpi(this).PixelsPerDip);
            return new(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
        }
    }
}
