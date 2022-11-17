using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary.UIs
{
    public interface IPageController
    {
        public int Id { get; set; }
        public string Name { get; set; }
        bool TryFindExhibition(string name, out IExhibition? exhibition);
        bool TryFindExhibition(int id, out IExhibition? exhibition);
        IExhibition TryAddExhibition(int id,string name, System.Drawing.Rectangle rectangle, int z);
        void ShowView();
        void ShowCode(string code);
        void ShowMessage(string message,TimeSpan delay);
        void SetSize(int width, int height);
        Size GetSize();
        void Close();
        void Clear();
    }
}
