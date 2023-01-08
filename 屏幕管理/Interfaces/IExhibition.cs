using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace 屏幕管理.Interfaces
{
    public interface IExhibition
    {
        int Id { get; init; }
        string Name { get; set; }
        void SetClock(Enums.ClockTypeEnum clockType, Models.BaseText text);
        void SetText(Models.BaseText text);
        void SetWeb(string? url);
        bool ShowText(Models.BaseText text, int playtime = 15);
        bool ShowAudio(string source, int playtime = 15);
        bool ShowVideo(string source, bool mute, int playtime = 15);
        bool ShowImage(string source, int playtime = 15);
        bool ShowWeb(string? url, int playtime = 15);
        bool ShowDownload(string title, Controllers.DownloadController.DownloadTask download, int playtime = 15);
    }
}
