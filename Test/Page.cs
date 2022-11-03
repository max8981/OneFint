using ClientLibrary.UIs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Page : ClientLibrary.UIs.IPageController
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void ShowCode(string code)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message, int time)
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

        public IExhibition TryAddExhibition(int id, string name, Rectangle rectangle, int z)
        {
            return new Exhibition();
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
