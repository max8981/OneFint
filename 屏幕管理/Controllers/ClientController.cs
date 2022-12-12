using 屏幕管理.ClientToServer;
using 屏幕管理.ServerToClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace 屏幕管理.Controllers
{
    internal partial class ClientController
    {
        private readonly Mqtt _mqtt;
        private readonly IClientController _client;
        private readonly ClientConfig _clientConfig;
        private readonly ILayoutWindow[] _layouts;
        private readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        public ClientController(IClientController client,ClientConfig clientConfig, ILayoutWindow[] layouts)
        {
            _mqtt = new("", 0);
            _layouts = layouts;
            _exhibitions = new Dictionary<int, ExhibitionController>();
            _clientConfig = clientConfig;
            _mqtt.Receive = (t, j) => ReceiveTopics(t, j);
        }
        public void Connect()
        {
            _mqtt.ConnectAsync();
        }
        private void ReceiveTopics(ServerToClient.TopicTypeEnum topic, string json)
        {
            switch (topic)
            {
                case ServerToClient.TopicTypeEnum.delete_material:
                    if (TryFromJson<ServerToClient.DeleteMaterial>(json, out var deleteMaterial))
                        DeleteMaterial(deleteMaterial);
                    break;
                case ServerToClient.TopicTypeEnum.material_download_url:
                    if (TryFromJson<ServerToClient.MaterialDownloadUrl>(json, out var materialDownloadUrl))
                        MaterialDownloadUrl(materialDownloadUrl);
                    break;
                case ServerToClient.TopicTypeEnum.delete_new_flash_content:
                    if (TryFromJson<ServerToClient.DeleteNewFlashContent>(json, out var deleteNewFlashContent))
                        DeleteNewFlashContent(deleteNewFlashContent);
                    break;
                case ServerToClient.TopicTypeEnum.new_flash_content:
                    if (TryFromJson<ServerToClient.BaseContent>(json, out var newFlashContent))
                        NewFlashContent(newFlashContent);
                    break;
                case ServerToClient.TopicTypeEnum.content:
                    if (TryFromJson<ServerToClient.BaseContent>(json, out var normalContent))
                        NormalAndDefaultContent(normalContent);
                    break;
                case ServerToClient.TopicTypeEnum.emergency_content:
                    if (TryFromJson<ServerToClient.BaseContent>(json, out var emergencyContent))
                        EmergencyContent(emergencyContent);
                    break;
                case ServerToClient.TopicTypeEnum.timing_boot:
                    if (TryFromJson<ServerToClient.TimingBoot>(json, out var timingBoot))
                    {
                        _timingBoots.Clear();
                        if (timingBoot != null)
                            if (timingBoot.Policies != null)
                                foreach (var policy in timingBoot.Policies)
                                    _timingBoots.TryAdd(policy.Id, policy);
                    }
                    break;
                case ServerToClient.TopicTypeEnum.timing_volume:
                    if (TryFromJson<ServerToClient.TimingVolume>(json, out var timingVolume))
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
                    if (TryFromJson<ServerToClient.ScreenControl>(json, out var screen))
                        ScreenControl(screen);
                    break;
                case ServerToClient.TopicTypeEnum.time_sync:
                    if (TryFromJson<ServerToClient.TimeSync>(json, out var timeSync))
                        TimeSync(timeSync);
                    break;
            }
        }

        private bool TryFromJson<T>(string json, out T result) where T : ServerToClient.ServerTopic,new()
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json) ?? new();
                return result?.Code == _clientConfig.Code;
            }
            catch (Exception ex)
            {
                result = new T();
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private bool TryGetOrder(string json, out Models.Order order)
        {
            var result = JsonSerializer.Deserialize<ServerToClient.ClientControl>(json)??new();
            order = result.Order ?? new();
            return result.Order?.Code == _clientConfig.Code;
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
                    _client.ScreenActivation(true);
                    break;
                case Enums.OrderEnum.SCREEN_SHUTDOWN:
                    _client.ScreenActivation(false);
                    break;
                case Enums.OrderEnum.CLEAN_UP_CACHE:
                    _client.DeleteFiles(System.IO.Directory.GetFiles(_clientConfig.MaterialPath));
                    break;
                case Enums.OrderEnum.VOLUME_CONTROL:
                    var vol = (int)Math.Clamp(order.Volume * 6.66, 0, 100);
                    _client.SetVolume(vol);
                    break;
            }
        }
        private void ScreenControl(ScreenControl screen)
        {
            _client.ScreenActivation(screen.ScreenActivation);
            if (screen.ScreenActivation)
                StartAll();
            else
                StopAll();
            DeviceInfo(screen.ScreenActivation);
        }
        private void TimeSync(TimeSync timeSync)
        {
            var date = DateTime.Now.AddSeconds(timeSync.ForwardSecond);
            _client.SetDate(date);
        }
        private void DeviceInfo(bool activation, bool refreshContent = false)
        {
            //DeviceInfo info = new()
            //{
            //    ScreenActivation = activation,
            //    RefreshContent = refreshContent,
            //};
            //_client.Send(ClientToServer.TopicTypeEnum.device_info.ToString(), JsonSerializer.Serialize(info));
        }
        public void Save()
        {
            foreach (var item in GetType().GetProperties())
            {
                //var name = item.Name;
                //var value = item.GetValue(this);
                //if (value != null)
                //    _client.SaveConfiguration(name, value.ToString()!);
            }
        }
    }
}
