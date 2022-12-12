using ClientCore;
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
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// LayoutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LayoutWindow : Window,ClientCore.ILayoutWindow
    {
        public LayoutWindow()
        {
            InitializeComponent();
        }

        public int Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public System.Drawing.Size GetSize()
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public void ShowView()
        {
            throw new NotImplementedException();
        }

        public IExhibition TryAddExhibition(int id, string name, System.Drawing.Rectangle rectangle, int z)
        {
            throw new NotImplementedException();
        }

        public bool TryFindExhibition(string name, out IExhibition? exhibition)
        {
            throw new NotImplementedException();
        }

        public bool TryFindExhibition(int id, out IExhibition? exhibition)
        {
            throw new NotImplementedException();
        }
    }
}
