using ClientLibrary.ClientToServer;
using ClientLibrary.ServerToClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ClientLibrary
{
    public class ClientController
    {
        const string CODE = "max01";
        private readonly System.Timers.Timer _timer;
        private static readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private static readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        public ClientController()
        {
            _timer = new(1000) { AutoReset = true };
            _timer.Elapsed += (o, e) =>
            {
                var now = e.SignalTime;
                TimingBoot(now);
                TimingVolume(now);
                if (now.Second % 10 == 0)
                    HeartBeat(new ClientToServer.HeartBeat(CODE));
            };
            _timer.Start();
        }
        internal static void DeleteMaterial(ServerToClient.DeleteMaterial delete)
        {
            if(delete.Materials!=null)
                foreach (var material in delete.Materials)
                    if (material.Content != null)
                    {
                        var uri = new Uri(material.Content);
                        var id = material.Id;
                        var ext = Path.GetExtension(uri.Segments.Last());
                        var file = $"{id}.{ext}"; 
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            WriteLog("DeleteMaterial", ex.Message);
                        }
                    }
        }
        internal static void MaterialDownloadUrl(ServerToClient.MaterialDownloadUrl material)
        {
            var id = material.ContentId;
            var deviceId = material.DeviceId;
            var deviceGroupId = material.DeviceGroupId;
            if (!string.IsNullOrEmpty(material.Url))
            {
                var url = material.Url;
                var task = Downloader.GetOrAddTask(id, url);
                task.CompleteCallback += o => MaterialDownloadStatus(new ClientToServer.MaterialDownloadStatus(id, true, deviceId, deviceGroupId));
            }
        }
        internal static void AddTimingBootPolicy(Models.TimingBootPolicy policy)
        {
            _timingBoots.TryAdd(policy.Id, policy);
        }
        internal static void AddTimingVolumePolicy(Models.TimingVolumePolicy policy)
        {
            _timingVolumes.TryAdd(policy.Id, policy);
        }
        private static void TimingBoot(DateTime now)
        {
            foreach (var timingboot in _timingBoots.Values)
            {
                _ = DateTime.TryParse(timingboot.StartedAt, out var start);
                _ = DateTime.TryParse(timingboot.EndedAt, out var end);
                if(end.TimeOfDay.TotalSeconds== now.TimeOfDay.TotalSeconds)
                {
                    switch (timingboot.LoopType)
                    {
                        case Enums.LoopTypeEnum.EVERYDAY:
                            PowerOn(start);
                            PowerOff(now);
                            break;
                        case Enums.LoopTypeEnum.SOMEDAY:
                            if (timingboot.Weekdays != null)
                                if(timingboot.Weekdays.Days!=null)
                                    if (timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                                    {
                                        PowerOn(start);
                                        PowerOff(now);
                                    }
                            break;
                        default:
                            PowerOn(start);
                            PowerOff(now);
                            break;
                    }
                }
            }
        }
        private static void TimingVolume(DateTime now)
        {
            foreach (var timingVolume in _timingVolumes.Values)
            {
                _ = DateTime.TryParse(timingVolume.StartedAt, out var start);
                _ = DateTime.TryParse(timingVolume.EndedAt, out var end);
                if (end.TimeOfDay.TotalSeconds == now.TimeOfDay.TotalSeconds)
                {
                    var vol = (int)Math.Clamp(timingVolume.Volume * 6.66, 0, 100);
                    SetVolume(vol);
                }
            }
        }
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
        public static Action<HeartBeat> HeartBeat { get; set; } = o => { };
        public static Action<MaterialDownloadStatus> MaterialDownloadStatus { get; set; } = o => { };
        ~ClientController()
        {
            _timer.Close();
        }
    }          
}
