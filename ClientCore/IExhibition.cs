using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace ClientCore
{
    public interface IExhibition
    {
        int Id { get; init; }
        string Name { get; set; }
        void ShowText(int id, string? text, string? color, int size = 0, string? bgcolor = null, int horizontal = 0, int vertical = 0);
        void ShowAudio(int id, string source);
        void ShowVideo(int id,string source, bool mute);
        void ShowImage(int id,string source);
        void ShowWeb(int id, string? url);
        void ShowClock(int id, int clockType, string? text, string? color, int size = 0, string? bgcolor = null, int horizontal = 0, int vertical = 0);
        void ShowDownload(int id,string title,string content,float progress);
        void Hidden(int id = -1); 
    }
}
