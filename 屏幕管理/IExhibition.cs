using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace 屏幕管理
{
    public interface IExhibition
    {
        int Id { get; init; }
        string Name { get; set; }
        void ShowClock(int clockType, string? color, int size = 0, string? bgcolor = null);
        bool ShowText(string? text, string? color, int size = 0, string? bgcolor = null, int horizontal = 0, int vertical = 0, int playtime = 15);
        bool ShowAudio(string source, int playtime = 15);
        bool ShowVideo(string source, bool mute, int playtime = 15);
        bool ShowImage(string source, int playtime = 15);
        bool ShowWeb(string? url, int playtime = 15);
        bool ShowDownload(string title, IDownload download, int playtime = 15);
    }
}
