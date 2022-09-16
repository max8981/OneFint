using ClientLibrary.ClientToServer;
using ClientLibrary.ServerToClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientLibrary
{
    public class ClientController
    {
        public static Action<DateTime> PowerOn { get; set; } = o => { };
        public static Action<DateTime> PowerOff { get; set; } = o => { };
        public static Action Reboot { get; set; } = () => { };
        public static Action<float> SetVolume { get; set; } = o => { };
        public static Action<int> ScreenPowerOff { get; set; } = o => { };
        public static Action<int> ScreenPowerOn { get; set; } = o => { };
        public static Action<int> ScreenBrightness { get; set; } = o => { };
        public static Action Save { get; set; } = () => { };
        public static Action Load { get; set; } = () => { };
        public static Action<string,string> WriteLog { get; set; } = (t,c) => { };
        public static Action<DeleteMaterial> DeleteMaterial { get; set; } = o => { };
        public static Action<DeleteNewFlashContent> DeleteNewFlashContent { get; set; } = o => { };
        public static Func<MaterialDownloadUrl, bool> MaterialDownloadUrl { get; set; } = o => { return false; };
        public static Action<BaseContent> NewFlashContent { get; set; } = o => { };
        public static Action<BaseContent> NormalAndDefaultContent { get; set; } = o => { };
        public static Action<BaseContent> EmergencyContent { get; set; } = o => { };
        public static Action<TimingBoot> TimingBoot { get; set; } = o => { };
        public static Action<TimingVolume> TimingVolume { get; set; } = o => { };
        public static Action<HeartBeat> HeartBeat { get; set; } = o => { };
        public static Action<MaterialDownloadStatus> MaterialDownloadStatus { get; set; } = o => { };
    }          
}
