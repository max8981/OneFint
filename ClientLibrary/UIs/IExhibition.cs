using ClientLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace ClientLibrary.UIs
{
    public interface IExhibition
    {
        int Id { get; init; }
        string Name { get; set; }
        void ShowText(int id,BaseText text);
        void ShowAudio(int id, string source);
        void ShowVideo(int id,string source, bool mute);
        void ShowImage(int id,string source);
        void ShowWeb(int id, string url);
        void ShowClock(int id, BaseText text, Enums.ClockTypeEnum clockType);
        void ShowDownload(int id,string title,string content,float progress);
        void Hidden(int id = -1); 
    }
}
