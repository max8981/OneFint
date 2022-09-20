using ClientLibrary.Models;
using ClientLibrary.ServerToClient;
using ClientLibrary.UIs;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ClientLibrary
{
    public class ClientConnect:ClientController
    {
        private readonly string _code;
        private readonly IMqttClient _client;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private readonly UIController _UI;
        private bool _connected = false;
        public ClientConnect(ClientConfig config,IPageController page)
        {
            _UI = new(page);
            _code = config.Code;
            _client = new MqttFactory().CreateMqttClient();
            _client.ConnectingAsync += _ =>
            {
                WriteLog("Mqtt", "Connecting");
                return Task.CompletedTask;
            };
            _client.ConnectedAsync += _ =>
            {
                WriteLog("Mqtt", _.ConnectResult.ResultCode.ToString());
                Subscribe();
                HeartBeat(new ClientToServer.HeartBeat(_code));
                _connected = true;
                return Task.CompletedTask;
            };
            _client.ApplicationMessageReceivedAsync += arg =>
            {
                var topic = (ServerToClient.TopicTypeEnum)Enum.Parse(typeof(ServerToClient.TopicTypeEnum), arg.ApplicationMessage.Topic);
                var json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                Receive(topic, json);
                return Task.CompletedTask;
            };
            _client.DisconnectedAsync += _ =>
            {
                ClientController.WriteLog("Mqtt", _.Reason.ToString());
                _connected = false;
                return Task.CompletedTask;
            };
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithWebSocketServer(config.Server)
                .WithCredentials(config.MqttUser, config.MqttPassword);
            HeartBeat = o => 
            Send(ClientToServer.TopicTypeEnum.heartbeat, JsonSerializer.Serialize(o));
            MaterialDownloadStatus = o => 
            Send(ClientToServer.TopicTypeEnum.material_download_status, JsonSerializer.Serialize(o));
            WriteLog = WrietLog;
            //Task.Factory.StartNew(() => Web.WebServer.Start(Array.Empty<string>()));
            Task.Factory.StartNew(() => WebServer.Program.Main(Array.Empty<string>()));
        }
        public async void Conntect()
        {
            try
            {
                await _client.ConnectAsync(_optionsBuilder.Build());
            }
            catch (Exception ex)
            {
                ClientController.WriteLog("Mqtt", ex.Message);
            }
        }
        public void Close()
        {
            _client.Dispose();
        }
        private async void Subscribe()
        {
            foreach (var topic in Enum.GetNames(typeof(ServerToClient.TopicTypeEnum)))
            {
                await _client.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
            }
        }
        private void Receive(ServerToClient.TopicTypeEnum topic, string json)
        {
            WriteLog(topic.ToString(), json);
            switch (topic)
            {
                case ServerToClient.TopicTypeEnum.delete_material:
                    if (TryFromJson<DeleteMaterial>(json, out var deleteMaterial))
                        DeleteMaterial(deleteMaterial!);
                    break;
                case ServerToClient.TopicTypeEnum.delete_new_flash_content:
                    if (TryFromJson<DeleteNewFlashContent>(json, out var deleteNewFlashContent))
                        _UI.DeleteNewFlashContent(deleteNewFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.material_download_url:
                    if (TryFromJson<MaterialDownloadUrl>(json, out var materialDownloadUrl))
                        MaterialDownloadUrl(materialDownloadUrl!);
                    break;
                case ServerToClient.TopicTypeEnum.new_flash_content:
                    if (TryFromJson<BaseContent>(json, out var newFlashContent))
                        _UI.NewFlashContent(newFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.content:
                    if (TryFromJson<BaseContent>(json, out var normalContent))
                        _UI.NormalAndDefaultContent(normalContent!);
                    break;
                case ServerToClient.TopicTypeEnum.emergency_content:
                    if (TryFromJson<BaseContent>(json, out var emergencyContent))
                        _UI.EmergencyContent(emergencyContent!);
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
        private async void Send(ClientToServer.TopicTypeEnum topic, string json)
        {
            if (_connected)
                await _client.PublishStringAsync(topic.ToString(), json);
            else
                Conntect();
            //ClientController.WriteLog(topic.ToString(), json);
        }
        private static void WrietLog(string title, string content)
        {
            var path = $"./log/{DateTime.Now:M}/";
            var text = $"\n{DateTime.Now}\n{content}\r";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.AppendAllText($"{path}{title}.txt", text);
            Console.WriteLine($"{DateTime.Now}\t[{title}]\t{content}");
        }
        private bool TryFromJson<T>(string json,out T? result)where T:Topic
        {
            result = JsonSerializer.Deserialize<T>(json);
            if (result?.Code == _code)
            {
                return true;
            }
            return false;
        }
        ~ClientConnect()
        {
            Close();
        }
    }
}
