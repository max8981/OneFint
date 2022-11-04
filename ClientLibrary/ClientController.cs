using ClientLibrary.ClientToServer;
using ClientLibrary.Models;
using ClientLibrary.ServerToClient;
using ClientLibrary.UIs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
namespace ClientLibrary
{
    public class ClientController
    {
        private readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        private readonly IClient _client;
        private readonly UIs.UIController _uIController;
        private readonly System.Timers.Timer _timer;
        private readonly Mqtt _mqtt;
        private int _lastvol;
        public ClientController(IClient client)
        {
            Downloader.SendDownloadStatus = MaterialDownloadStatus;
            _client = client;
            _uIController = new(client.PageControllers);
            _mqtt = new(_client)
            {
                Receive = ReceiveTopics
            };
            _timer = new(1000) { AutoReset = true };
            int t = 0;
            _timer.Elapsed += (o, e) =>
            {
                var now = e.SignalTime;
                t++;
                TimingBoot(now);
                TimingVolume(now);
                if (t == _client.Config.HeartBeatSecond)
                {
                    HeartBeat();
                    t = 0;
                }
                if (_client.Config.AutoReboot && (int)now.TimeOfDay.TotalSeconds == 3 * 60 * 60)
                    Task.Factory.StartNew(async () =>
                    {
                        await Task.Delay(new Random().Next(0, 3600));
                        _client.Reboot();
                    });
            };
            _mqtt.Connected = () =>
            {
                Reconnect();
            };
            Connect();
            _timer.Start();
        }
        private void Connect()
        {
            _mqtt.Connect();
        }
        private void ReceiveTopics(ServerToClient.TopicTypeEnum topic, string json)
        {
            _client.WriteLog(topic.ToString(), json);
#if DEBUG
            //System.IO.File.WriteAllText($"./log/{topic}-{DateTime.Now:HH-mm-ss-FF}.txt", json);
#endif
            switch (topic)
            {
                case ServerToClient.TopicTypeEnum.delete_material:
                    if (TryFromJson<DeleteMaterial>(json, out var deleteMaterial))
                        DeleteMaterial(deleteMaterial!);
                    break;
                case ServerToClient.TopicTypeEnum.delete_new_flash_content:
                    if (TryFromJson<DeleteNewFlashContent>(json, out var deleteNewFlashContent))
                        _uIController.DeleteNewFlashContent(deleteNewFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.material_download_url:
                    if (TryFromJson<MaterialDownloadUrl>(json, out var materialDownloadUrl))
                        MaterialDownloadUrl(materialDownloadUrl!);
                    break;
                case ServerToClient.TopicTypeEnum.new_flash_content:
                    if (TryFromJson<BaseContent>(json, out var newFlashContent))
                        _uIController.NewFlashContent(newFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.content:
                    if (TryFromJson<BaseContent>(json, out var normalContent))
                        _uIController.NormalAndDefaultContent(normalContent!);
                    break;
                case ServerToClient.TopicTypeEnum.emergency_content:
                    if (TryFromJson<BaseContent>(json, out var emergencyContent))
                        _uIController.EmergencyContent(emergencyContent!);
                    break;
                case ServerToClient.TopicTypeEnum.timing_boot:
                    if (TryFromJson<TimingBoot>(json, out var timingBoot))
                    {
                        _timingBoots.Clear();
                        if (timingBoot != null)
                            if (timingBoot.Policies != null)
                                foreach (var policy in timingBoot.Policies)
                                    AddTimingBootPolicy(policy);
                    }
                    break;
                case ServerToClient.TopicTypeEnum.timing_volume:
                    if (TryFromJson<TimingVolume>(json, out var timingVolume))
                    {
                        _timingVolumes.Clear();
                        if (timingVolume != null)
                            if (timingVolume.Policies != null)
                                foreach (var policy in timingVolume.Policies)
                                    AddTimingVolumePolicy(policy);
                    }
                    break;
                case ServerToClient.TopicTypeEnum.order:
                    if (TryGetOrder(json, out var order))
                        if (order != null)
                            Order(order);
                    break;
            }
        }
        internal void DeleteMaterial(ServerToClient.DeleteMaterial delete)
        {
            if(delete.Materials!=null)
                foreach (var material in delete.Materials)
                    if (material.Content != null)
                    {
                        var uri = new Uri(material.Content);
                        var id = material.Id;
                        var ext = Path.GetExtension(uri.Segments.Last());
                        var file = Path.Combine(_client.Config.MaterialPath, $"{id}{ext}");
                        DeleteFiles(new string[] { file });
                    }
            if (delete.DeleteAll)
            {
                var files = System.IO.Directory.GetFiles(_client.Config.MaterialPath);
                DeleteFiles(files);
            }
        }
        internal void MaterialDownloadUrl(ServerToClient.MaterialDownloadUrl material)
        {
            var id = material.ContentId;
            var deviceId = material.DeviceId;
            var deviceGroupId = material.DeviceGroupId;
            if (!string.IsNullOrEmpty(material.Url))
            {
                var url = material.Url;
                //var task = Downloader.GetOrAddTask(id, url,id, deviceId, deviceGroupId);
                //task.CompleteCallback += o => MaterialDownloadStatus(new ClientToServer.MaterialDownloadStatus(id, true, deviceId, deviceGroupId));
            }
        }
        internal void AddTimingBootPolicy(Models.TimingBootPolicy policy)
        {
            _timingBoots.TryAdd(policy.Id, policy);
        }
        internal void AddTimingVolumePolicy(Models.TimingVolumePolicy policy)
        {
            _timingVolumes.TryAdd(policy.Id, policy);
        }
        private void TimingBoot(DateTime now)
        {
            foreach (var timingboot in _timingBoots.Values)
            {
                _ = DateTime.TryParse(timingboot.StartedAt, out var start);
                _ = DateTime.TryParse(timingboot.EndedAt, out var end);
                if(end.TimeOfDay.TotalSeconds== (int)now.TimeOfDay.TotalSeconds)
                {
                    start = DateTime.Today.AddSeconds(start.TimeOfDay.TotalSeconds);
                    switch (timingboot.LoopType)
                    {
                        case Enums.LoopTypeEnum.EVERYDAY:
                            _client.PowerOn(start);
                            break;
                        case Enums.LoopTypeEnum.SOMEDAY:
                            if (timingboot.Weekdays != null)
                                if(timingboot.Weekdays.Days!=null)
                                    if (timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                                    {
                                        _client.PowerOn(start);
                                    }
                            break;
                        default:
                            _client.PowerOn(start);
                            break;
                    }
                }
            }
        }
        private void TimingVolume(DateTime now)
        {
            foreach (var timingVolume in _timingVolumes.Values)
            {
                _ = DateTime.TryParse(timingVolume.StartedAt, out var start);
                _ = DateTime.TryParse(timingVolume.EndedAt, out var end);
                if ((int)start.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                {
                    _lastvol = _client.Volume;
                    var vol = (int)Math.Clamp(timingVolume.Volume * 6.66, 0, 100);
                    _client.SetVolume(vol);
                }
                if ((int)end.TimeOfDay.TotalSeconds == (int)now.TimeOfDay.TotalSeconds)
                {
                    _client.SetVolume(_lastvol);
                }
            }
        }
        private void Order(Models.Order order)
        {
            switch (order.OrderEnum)
            {
                case Enums.OrderEnum.SHUTDOWN:
                    _client.ShutDown();
                    break;
                case Enums.OrderEnum.REBOOT:
                    _client.Reboot();
                    break;
                case Enums.OrderEnum.SCREEN_BOOT:
                    _client.ScreenPowerOn(0);
                    break;
                case Enums.OrderEnum.SCREEN_SHUTDOWN:
                    _client.ScreenPowerOff(0);
                    break;
                case Enums.OrderEnum.CLEAN_UP_CACHE:
                    _client.DeleteFiles(System.IO.Directory.GetFiles(_client.Config.MaterialPath));
                    break;
                case Enums.OrderEnum.VOLUME_CONTROL:
                    var vol = (int)Math.Clamp(order.Volume * 6.66, 0, 100);
                    _client.SetVolume(vol);
                    break;
            }
        }
        private void HeartBeat()
        {
            _mqtt.Send(ClientToServer.TopicTypeEnum.heartbeat, JsonSerializer.Serialize(new ClientToServer.HeartBeat(_client.Config.Code)));
        }
        private void MaterialDownloadStatus(MaterialDownloadStatus status)
        {
            _mqtt.Send(ClientToServer.TopicTypeEnum.material_download_status, JsonSerializer.Serialize(status));
        }
        private void Reconnect()
        {
            _mqtt.Send(ClientToServer.TopicTypeEnum.reconnect, JsonSerializer.Serialize(new ClientToServer.Reconnect(_client.Config.Code)));
        }
        private bool TryFromJson<T>(string json, out T? result) where T : Topic
        {
            result = null;
            try
            {
                result = JsonSerializer.Deserialize<T>(json);
                return result?.Code == _client.Config.Code;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private bool TryGetOrder(string json,out Models.Order? order)
        {
            var result = JsonSerializer.Deserialize<ServerToClient.ClientControl>(json);
            order = result?.Order;
            return order?.Code == _client.Config.Code;
        }
        private void DeleteFiles(string[] files)
        {
            foreach (var file in files)
            {
                try
                {
                    System.IO.File.Delete(file);
                }
                catch
                {
                    _client.DeleteFiles(new string[] { file });
                }
            }
        }
        public void Close()
        {
            _timer.Close();
            _mqtt.Close();
            _uIController.Close();
        }
        ~ClientController()
        {
            Close();
        }
    }          
}
