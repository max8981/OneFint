using ClientLibrary.Enums;
using ClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Exhibition : ClientLibrary.UIs.IExhibition
    {
        public int Id { get; init; }
        public string Name { get; set; } = "";

        public void Hidden(int id = -1)
        {

        }

        public void ShowAudio(int id, string source)
        {

        }

        public void ShowClock(int id, BaseText? text, ClockTypeEnum clockType)
        {

        }

        public void ShowDownload(int id, string title, string content, float progress)
        {

        }

        public void ShowImage(int id, string source)
        {

        }

        public void ShowText(int id, BaseText? text)
        {

        }

        public void ShowVideo(int id, string source, bool mute)
        {

        }

        public void ShowWeb(int id, string? url)
        {

        }
    }
}
