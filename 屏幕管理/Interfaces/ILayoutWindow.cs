using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace 屏幕管理.Interfaces
{
    internal interface ILayoutWindow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        Controllers.ExhibitionController TryAddExhibition(int id, string name, Rectangle rectangle, int z);
        void ShowView(double width, double height);
        void ShowView();
        Size GetSize();
        void Close();
        void Clear();
    }
}
