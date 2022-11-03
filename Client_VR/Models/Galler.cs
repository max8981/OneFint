using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_VR.Models
{
    internal class Galler
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public int PvCount { get; set; }
        public GallerProperty? Property { get; set; }
        public int LikeCount { get; set; }
        public GallerAuthor? Author { get; set; }
        public string? Remark { get; set; }
        public int Toped { get; set; }
    }
}
