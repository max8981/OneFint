using ClientCore.ClientToServer;
using ClientCore.ServerToClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ClientCore.Controllers
{
    internal partial class ClientController
    {
        private readonly IClient _client;
        private readonly ClientConfig _clientConfig;
        private readonly IPage[] _pages;
        private readonly ConcurrentDictionary<int, Models.TimingBootPolicy> _timingBoots = new();
        private readonly ConcurrentDictionary<int, Models.TimingVolumePolicy> _timingVolumes = new();
        public ClientController(IClient client, IPage[] pages)
        {
            _client = client;
            _pages = pages;
            _exhibitions = new Dictionary<int, ExhibitionController>();
            _clientConfig = Load();
        }
        public void Connect()
        {
            _client.Connect();
        }
        private void ReceiveTopics(ServerToClient.TopicTypeEnum topic, string json)
        {
#if DEBUG
            //System.IO.File.WriteAllText($"./log/{topic}-{DateTime.Now:HH-mm-ss-FF}.txt", json);
#endif
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
            DeviceInfo info = new()
            {
                ScreenActivation = activation,
                RefreshContent = refreshContent,
            };
            _client.Send(ClientToServer.TopicTypeEnum.device_info.ToString(), JsonSerializer.Serialize(info));
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
        public ClientConfig Load()
        {
            ClientConfig config = new ClientConfig()
            {

            };
            //foreach (var item in config.GetType().GetProperties())
            //{
            //    var name = item.Name;
            //    var value = item.GetValue(this);
            //    if (value != null)
            //        _client.SaveConfiguration(name, value.ToString()!);
            //}
            //config.Code = _client.LoadConfiguration(nameof(Code));
            //config.MqttServer = _client.LoadConfiguration(nameof(MqttServer));
            //if (int.TryParse(_client.LoadConfiguration(nameof(MqttPort)), out var port))
            //    config.MqttPort = port;
            //config.MqttUser = _client.LoadConfiguration(nameof(MqttUser));
            //config.MqttPassword = _client.LoadConfiguration(nameof(MqttPassword));
            //config.HeartBeatSecond = int.TryParse(_client.LoadConfiguration(nameof(HeartBeatSecond)), out var number) ? number : 10;
            //config.MaterialPath = _client.LoadConfiguration(nameof(MaterialPath));
            //config.DelayedUpdate = !bool.TryParse(_client.LoadConfiguration(nameof(DelayedUpdate)), out var delayeUpdate) || delayeUpdate;
            //config.ShowDownloader = !bool.TryParse(_client.LoadConfiguration(nameof(ShowDownloader)), out var showDownloader) || showDownloader;
            //config.UpdateUrl = _client.LoadConfiguration(nameof(UpdateUrl));
            //config.GuardInterval = int.TryParse(_client.LoadConfiguration(nameof(GuardInterval)), out var interval) ? interval : 30;
            return config;
        }
    }
}
