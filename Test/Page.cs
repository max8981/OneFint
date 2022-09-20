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
