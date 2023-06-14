using 屏幕管理.ClientToServer;
using 屏幕管理.ServerToClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using 屏幕管理.Interfaces;
using System.Threading.Tasks;
using CefSharp.DevTools.CSS;
using 屏幕管理.Systems;

namespace 屏幕管理.Controllers
{
    internal partial class ClientController
    {
        private readonly ClientMqtt _mqtt;
        private readonly System.Timers.Timer _timer;
        private readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        private bool _manualScreen;
        private bool? _screenStatus = null;
        public ClientController(System.Windows.Forms.Screen[] screens)
        {
            Global.MaterialStatus = MaterialDownloadStatus;
            _mqtt = new(Config.MqttServer, Config.MqttPort, Config.MqttUser, Config.MqttPassword);
            _screens = new Dictionary<System.Windows.Forms.Screen, ILayoutWindow>();
            foreach (var screen in screens)
                _screens.Add(screen, new LayoutWindow(screen));
            _exhibitions = new Dictionary<int, ExhibitionController>();
            _mqtt.Receive = (t, j) => ReceiveTopics(t, j);
            _mqtt.Connected = () =>
            {
                HeartBeat();
                Task.Delay(3000).Wait();
                DeviceInfo(!HasContent);
                UploadLog();
                if (!HasContent)
                    Global.ShowCode(Config.Code);
            };
            _timer = new(1000) { AutoReset = true };
            int t = 0;
            _timer.Elapsed += (o, e) =>
            {
                t++;
                if (Config.FakeShutdown)
                    TimingBootFake(Global.Now);
                else
                    TimingBoot(Global.Now);
                TimingVolume(Global.Now);
                if (t >= Config.HeartBeatSecond)
                {
                    HeartBeat();
                    t = 0;
                }
                //if (Config.AutoReboot && (int)Global.Now.TimeOfDay.TotalSeconds == 3 * 60 * 60)
                //    System.Threading.Tasks.Task.Factory.StartNew(async () =>
                //    {
                //        await System.Threading.Tasks.Task.Delay(new Random().Next(0, 3600));
                //        Systems.Power.Reboot();
                //    });
            };
            _timer.Start();
        }
        public void Connect()
        {
            _mqtt.ConnectAsync();
        }
        public async Task<bool> ReConnect(string server, int port)
        {
            return await _mqtt.ReConnectAsync(server,port);
        }
        private void ReceiveTopics(ServerToClient.TopicTypeEnum topic, string json)
        {
            switch (topic)
            {
                case ServerToClient.TopicTypeEnum.delete_material:
                    if (TryFromJson<DeleteMaterial>(json, out var deleteMaterial))
                        DeleteMaterial(deleteMaterial);
                    break;
                case ServerToClient.TopicTypeEnum.material_download_url:
                    if (TryFromJson<MaterialDownloadUrl>(json, out var materialDownloadUrl))
                        MaterialDownloadUrl(materialDownloadUrl);
                    break;
                case ServerToClient.TopicTypeEnum.delete_new_flash_content:
                    if (TryFromJson<DeleteNewFlashContent>(json, out var deleteNewFlashContent))
                        DeleteNewFlashContent(deleteNewFlashContent);
                    break;
                case ServerToClient.TopicTypeEnum.new_flash_content:
                    if (TryFromJson<BaseContent>(json, out var newFlashContent))
                        NewFlashContent(newFlashContent);
                    break;
                case ServerToClient.TopicTypeEnum.content:
                    if (TryFromJson<BaseContent>(json, out var normalContent))
                        NormalAndDefaultContent(normalContent);
                    break;
                case ServerToClient.TopicTypeEnum.emergency_content:
                    if (TryFromJson<BaseContent>(json, out var emergencyContent))
                        EmergencyContent(emergencyContent);
                    break;
                case ServerToClient.TopicTypeEnum.timing_boot:
                    if (TryFromJson<TimingBoot>(json, out var timingBoot))
                    {
                        _timingBoots.Clear();
                        if (timingBoot != null)
                            if (timingBoot.Policies != null)
                                foreach (var policy in timingBoot.Policies)
                                    _timingBoots.TryAdd(policy.Id, policy);
                    }
                    break;
                case ServerToClient.TopicTypeEnum.timing_volume:
                    if (TryFromJson<TimingVolume>(json, out var timingVolume))
                    {
                        _timingVolumes.Clear();
                        if (timingVolume != null)
                            if (timingVolume.Policies != null)
                                foreach (var policy in timingVolume.Policies)
                                    _timingVolumes.TryAdd(policy.Id, policy);
                    }
                    break;
                case ServerToClient.TopicTypeEnum.order:
                    if (TryGetOrder(json, out var order))
                        if (order != null)
                            Order(order);
                    break;
                case ServerToClient.TopicTypeEnum.screen_control:
                    if (TryFromJson<ScreenControl>(json, out var screen))
                        ScreenControl(screen);
                    break;
                case ServerToClient.TopicTypeEnum.time_sync:
                    if (TryFromJson<TimeSync>(json, out var timeSync))
                        TimeSync(timeSync);
                    break;
            }
            if (!System.IO.Directory.Exists("./record/"))
                System.IO.Directory.CreateDirectory("./record/");
            System.IO.File.WriteAllText($"./record/{topic}", json);
        }
        private static bool TryFromJson<T>(string json, out T outresult) where T : ServerToClient.ServerTopic,new()
        {
            try
            {
                outresult = JsonSerializer.Deserialize<T>(json) ?? new();
                return outresult.Code == Config.Code;
            }
            catch (Exception ex)
            {
                Systems.Log.Default.Error(ex, "TryFromJson", $"Parameter:{json}");
                outresult = new T();
                return false;
            }
        }
        private static bool TryGetOrder(string json, out Models.Order order)
        {
            var result = JsonSerializer.Deserialize<ServerToClient.ClientControl>(json)??new();
            order = result.Order ?? new();
            return result.Order?.Code == Config.Code;
        }
        private void DeleteMaterial(ServerToClient.DeleteMaterial deleteMaterial)
        {
            if (deleteMaterial.Materials != null)
                foreach (var material in deleteMaterial.Materials)
                    if (DownloadController.TryGetMaterialFilePath(material, out var file))
                        System.IO.File.Delete(file);
            if (deleteMaterial.DeleteAll)
                foreach (var file in DownloadController.MaterialFiles)
                    System.IO.File.Delete(file);
        }
        private void MaterialDownloadUrl(ServerToClient.MaterialDownloadUrl material)
        {
            var id = material.ContentId;
            var deviceId = material.DeviceId;
            var deviceGroupId = material.DeviceGroupId;
            if (!string.IsNullOrEmpty(material.Url))
            {
                var url = material.Url;
            }
        }
        private static void Order(Models.Order order)
        {
            switch (order.OrderEnum)
            {
                case Enums.OrderEnum.SHUTDOWN:
                    Systems.Power.ShutDown();
                    break;
                case Enums.OrderEnum.REBOOT:
                    Systems.Power.Reboot();
                    break;
                case Enums.OrderEnum.SCREEN_BOOT:
                    Systems.Screen.ScreenPowerOn();
                    break;
                case Enums.OrderEnum.SCREEN_SHUTDOWN:
                    Systems.Screen.ScreenPowerOff();
                    break;
                case Enums.OrderEnum.CLEAN_UP_CACHE:
                    break;
                case Enums.OrderEnum.VOLUME_CONTROL:
                    Systems.Audio.SetVolume(order.Volume * 6.66);
                    break;
            }
        }
        private void ScreenControl(ScreenControl screen)
        {
            _manualScreen = screen.ScreenActivation;
            Global.Pause(!screen.ScreenActivation);
            Systems.Screen.ScreenPower(screen.ScreenActivation);
            DeviceInfo(false);
            foreach (var item in _exhibitions)
                item.Value.Pause(!screen.ScreenActivation);
        }
        private static void TimeSync(TimeSync timeSync) => Systems.Time.SetDate(timeSync.ForwardSecond);
        private void HeartBeat()
        {
            var heartBeat = new ClientToServer.HeartBeat(Config.Code);
            var json = JsonSerializer.Serialize<object>(heartBeat,new JsonSerializerOptions(JsonSerializerDefaults.General));
            _mqtt.Send(ClientToServer.TopicTypeEnum.heartbeat, json);
        }
        private void MaterialDownloadStatus(int contentId,int? deviceId,int?deviceGroupId )
        {
            var status = new ClientToServer.MaterialDownloadStatus(contentId, true, deviceId, deviceGroupId);
            _mqtt.Send(ClientToServer.TopicTypeEnum.material_download_status, JsonSerializer.Serialize(status));
        }
        private void DeviceInfo(bool refreshContent)
        {
            var info = new ClientToServer.DeviceInfo(Config.Code)
            {
                AppVersion = Global.AppVersion,
                IpAddress = Global.IpAddress,
                MacAddress = Global.MacAddress,
                ScreenActivation = Screen.ScreenActivation,
                FreeDisk = Global.FreeDisk,
                Ram = Global.Ram,
                RefreshContent = refreshContent,// !HasContent,  
                Rom = Global.Rom,
                SystemType = Enums.SystemTypeEnum.Windows,
            };
            _mqtt.Send(ClientToServer.TopicTypeEnum.device_info, JsonSerializer.Serialize(info));
        }
        private void UploadLog()
        {
            //var filePath = Log.LogFilePath;
            //if (System.IO.File.Exists(filePath))
            //{
            //    UploadLog content = new(Config.Code)
            //    {
            //        Log = filePath.ToBase64(),
            //    };
            //    _mqtt.Send(ClientToServer.TopicTypeEnum.upload_log, JsonSerializer.Serialize(content));
            //}
        }
        private void TimingBoot(DateTime now)
        {
            foreach (var timingboot in _timingBoots.Values)
            {
                _ = DateTime.TryParse(timingboot.StartedAt, out var start);
                _ = DateTime.TryParse(timingboot.EndedAt, out var end);
                start = DateTime.Today.AddSeconds(start.TimeOfDay.TotalSeconds);
                if (end.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                {
                    switch (timingboot.LoopType)
                    {
                        case Enums.LoopTypeEnum.EVERYDAY:
                            Systems.Power.HibernateTo(start);
                            break;
                        case Enums.LoopTypeEnum.SOMEDAY:
                            if (timingboot.Weekdays != null)
                                if (timingboot.Weekdays.Days != null)
                                    if (timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                                        Systems.Power.HibernateTo(start);
                            break;
                        default:
                            Systems.Power.HibernateTo(start);
                            break;
                    }
                }
                //if (start.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                //{
                //    switch (timingboot.LoopType)
                //    {
                //        case Enums.LoopTypeEnum.EVERYDAY:
                //            Systems.Screen.PowerOn();
                //            break;
                //        case Enums.LoopTypeEnum.SOMEDAY:
                //            if (timingboot.Weekdays != null)
                //                if (timingboot.Weekdays.Days != null)
                //                    if (timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                //                        Systems.Screen.PowerOn();
                //            break;
                //        default:
                //            Systems.Screen.PowerOn();
                //            break;
                //    }
                //}
            }
        }
        private void TimingBootFake(DateTime now)
        {
            bool? isScreenOff = null;
            var nowTime = now.TimeOfDay;
            foreach (var timingboot in _timingBoots.Values)
            {
                if (timingboot.LoopType == Enums.LoopTypeEnum.SOMEDAY)
                {
                    if (timingboot.Weekdays != null)
                        if (timingboot.Weekdays.Days != null)
                            if (!timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                                continue;
                }
                _ = DateTime.TryParse(timingboot.StartedAt, out var start);
                _ = DateTime.TryParse(timingboot.EndedAt, out var end);
                var offTime = end.TimeOfDay;
                var onTime = start.TimeOfDay;
                var rebootTime = start.AddSeconds(3).TimeOfDay;
                if (_manualScreen) continue;
                var a = nowTime > offTime;
                var b = nowTime < onTime;
                isScreenOff = offTime < onTime ? a & b : a | b;
                if (!Config.AutoReboot && nowTime > onTime && nowTime < rebootTime)
                    Systems.Power.Reboot();
            }
            if (isScreenOff.HasValue)
            {
                if (isScreenOff.Value)
                {
                    if (Systems.Screen.ScreenActivation)
                    {
                        Global.MQTTLog?.Invoke("FakeShutdown", "执行关屏");
                        Systems.Screen.ScreenPowerOff();
                        FakeShutdown();
                    }
                }else if (!Systems.Screen.ScreenActivation)
                {
                    if (_manualScreen)
                        _manualScreen = false;
                    Global.MQTTLog?.Invoke("TimingBoot", "执行开屏");
                    Systems.Screen.ScreenPowerOn();
                    FakeShutdown();
                }
            }
        }
        private int _lastvol;
        private void TimingVolume(DateTime now)
        {
            foreach (var timingVolume in _timingVolumes.Values)
            {
                _ = DateTime.TryParse(timingVolume.StartedAt, out var start);
                _ = DateTime.TryParse(timingVolume.EndedAt, out var end);
                if ((int)start.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                {
                    _lastvol = Systems.Audio.GetVolume();
                    var vol = (int)Math.Clamp(timingVolume.Volume * 6.66, 0, 100);
                    Systems.Audio.SetVolume(vol);
                }
                if ((int)end.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                {
                    Systems.Audio.SetVolume(_lastvol);
                }
            }
        }
        private void FakeShutdown()
        {
            if (Config.FakeShutdown&& Screen.ScreenActivation!= _screenStatus)
            {
                _screenStatus = Screen.ScreenActivation;
                Global.Pause(!Screen.ScreenActivation);
                var info = new ClientToServer.DeviceInfo(Config.Code)
                {
                    AppVersion = Global.AppVersion,
                    IpAddress = Global.IpAddress,
                    MacAddress = Global.MacAddress,
                    ScreenActivation = Screen.ScreenActivation,
                    FreeDisk = Global.FreeDisk,
                    Ram = Global.Ram,
                    RefreshContent = false,// !HasContent,
                    Rom = Global.Rom,
                    SystemType = Enums.SystemTypeEnum.Windows,
                };
                _mqtt.Send(ClientToServer.TopicTypeEnum.device_info, JsonSerializer.Serialize(info));
            }
        }
    }
}
