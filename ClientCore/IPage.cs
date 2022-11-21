using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ClientCore
{
    internal interface IPage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        bool TryFindExhibition(string name, out IExhibition? exhibition);
        bool TryFindExhibition(int id, out IExhibition? exhibition);
        IExhibition TryAddExhibition(int id, string name, System.Drawing.Rectangle rectangle, int z);
        void ShowView();
        void ShowMessage(string message, TimeSpan delay);
        Size GetSize();
        void Close();
        void Clear();
    }
}
