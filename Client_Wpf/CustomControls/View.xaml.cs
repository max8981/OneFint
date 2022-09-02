using SharedProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_Wpf.CustomControls
{
    /// <summary>
    /// View.xaml 的交互逻辑
    /// </summary>
    public partial class View : Window
    {
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern void ShowCursor(int status);
        public View()
        {
            InitializeComponent();
            Background = new SolidColorBrush(Colors.Black);
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Topmost = true;
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Activated += (o, e) => { ShowCursor(0); };
            Deactivated+=(o,e) => { ShowCursor(1); };
        }
        public void SetNormalContents(NormalContent normal)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                grid.Children.Clear();
                if (normal.Layout.Content.Pages != null)
                {
                    foreach (var page in normal.Layout.Content.Pages)
                    {
                        foreach (var component in page.Components)
                        {
                            var control = new CustomControls.ExhibitionControl(component);
                            switch (component.ComponentType)
                            {
                                case Component.ComponentTypeEnum.BROWSER:
                                    grid.Children.Add(control.ShowWebBrowser(component.Text)); break;
                                case Component.ComponentTypeEnum.TEXT:
                                    grid.Children.Add(control.ShowText(component.Text)); break;
                                case Component.ComponentTypeEnum.CLOCK:
                                    grid.Children.Add(control.ShowClock(component)); break;
                            }
                        }
                    }
                }
                if(normal.DefaultContents != null)
                {
                    SetContents(normal.DefaultContents);
                }
                if (normal.NormalContents != null)
                {
                    SetContents(normal.NormalContents);
                }
                Visibility = Visibility.Visible;
            }));
        }
        public void SetEmergencyContent(EmergencyContent emergency)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (emergency.EmergencyContents != null)
                {
                    SetContents(emergency.EmergencyContents);
                }
            }));
        }
        public void SetNewFlashContent(NewFlashContent newFlash)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (newFlash.NewFlashContentPayloads != null)
                {
                    foreach (var payload in newFlash.NewFlashContentPayloads)
                    {
                        SetContents(new Content[] { payload.NewFlashContent });
                    }
                }
            }));
        }
        public void SetContents(Content[] contents)
        {
            foreach (var content in contents)
            {
                var control = new ExhibitionControl(content.Component);
                switch (content.Material.MaterialType)
                {
                    case Material.MaterialTypeEnum.MATERIAL_TYPE_IMAGE:
                        control = (ExhibitionControl)control.ShowImage(content); break;
                    case Material.MaterialTypeEnum.MATERIAL_TYPE_VIDEO:
                        control = (ExhibitionControl)control.ShowVideo(content); break;
                    case Material.MaterialTypeEnum.MATERIAL_TYPE_TEXT:
                        control = (ExhibitionControl)control.ShowText(content.Text); break;
                }
                grid.Children.Add(control);
            }
        }
        public Grid Grid => grid;
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                //Close();
                Visibility = Visibility.Hidden;
            }
        }
    }
}
