using ClientLibrary.ClientToServer;
using ClientLibrary.Models;
using ClientLibrary.ServerToClient;
using ClientLibrary.UIs;
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
        private readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        private readonly IClient _client;
        private readonly UIs.UIController _uIController;
        private readonly System.Timers.Timer _timer;
        private readonly Mqtt _mqtt;
        private int _lastvol;
        public ClientController(IClient client)
        {
            _client = client;
            _uIController = new(client.PageControllers);
            _mqtt = new(_client)
            {
                Receive = ReceiveTopics
            };
            _timer = new(1000) { AutoReset = true };
            _timer.Elapsed += (o, e) =>
            {
                var now = e.SignalTime;
                TimingBoot(now);
                TimingVolume(now);
                if (now.Second % 10 == 0)
                    HeartBeat();
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
                        if (timingBoot != null)
                            if (timingBoot.Policies != null)
                                foreach (var policy in timingBoot.Policies)
                                    AddTimingBootPolicy(policy);
                    break;
                case ServerToClient.TopicTypeEnum.timing_volume:
                    if (TryFromJson<TimingVolume>(json, out var timingVolume))
                        if (timingVolume != null)
                            if (timingVolume.Policies != null)
                                foreach (var policy in timingVolume.Policies)
                                    AddTimingVolumePolicy(policy);
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
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            _client.WriteLog("DeleteMaterial-Exception", ex.Message);
                        }
                    }
            if (delete.DeleteAll)
            {
                foreach (var file in System.IO.Directory.GetFiles(_client.Config.MaterialPath))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        _client.WriteLog("DeleteMaterial-Exception", ex.Message);
                    }
                }
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
                var task = Downloader.GetOrAddTask(id, url);
                task.CompleteCallback += o => MaterialDownloadStatus(new ClientToServer.MaterialDownloadStatus(id, true, deviceId, deviceGroupId));
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
                if(end.TimeOfDay.TotalSeconds== now.TimeOfDay.TotalSeconds)
                {
                    switch (timingboot.LoopType)
                    {
                        case Enums.LoopTypeEnum.EVERYDAY:
                            _client.PowerOn(start);
                            _client.PowerOff(now);
                            break;
                        case Enums.LoopTypeEnum.SOMEDAY:
                            if (timingboot.Weekdays != null)
                                if(timingboot.Weekdays.Days!=null)
                                    if (timingboot.Weekdays.Days.Contains((Enums.WeekdayEnum)now.DayOfWeek))
                                    {
                                        _client.PowerOn(start);
                                        _client.PowerOff(now);
                                    }
                            break;
                        default:
                            _client.PowerOn(start);
                            _client.PowerOff(now);
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
        private void HeartBeat()
        {
            _mqtt.Send(ClientToServer.TopicTypeEnum.heartbeat, JsonSerializer.Serialize(new ClientToServer.HeartBeat(_client.Config.Code)));
        }
        private void MaterialDownloadStatus(MaterialDownloadStatus status)
        {
            _mqtt.Send(ClientToServer.TopicTypeEnum.heartbeat, JsonSerializer.Serialize(status));
        }
        private bool TryFromJson<T>(string json, out T? result) where T : Topic
        {
            result = JsonSerializer.Deserialize<T>(json);
            if (result?.Code == _client.Config.Code)
            {
                return true;
            }
            return false;
        }
        public void Close()
        {
            _timer.Close();
            _mqtt.Close();
            _uIController.Close();
        }
        public void SetDelayedUpdate(bool b) => _uIController.SetDelayedUpdate(b);
        ~ClientController()
        {
            Close();
        }
    }          
}
