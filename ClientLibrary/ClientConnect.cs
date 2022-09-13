using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using SharedProject;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ClientLibrary
{
    public class ClientConnect
    {
        private readonly string _code;
        private readonly System.Timers.Timer _timer;
        private readonly IMqttClient _client;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private bool _connected = false;
        public ClientConnect(ClientConfig config)
        {
            _code = config.Code;
            _timer = new System.Timers.Timer
            {
                Interval = 3000,
                AutoReset = true
            };
            _timer.Elapsed += (o, e) =>
            {
                if (_connected)
                {
                    ClientController.HeartBeat(new ClientToServer.HeartBeat(_code));
                }
                else
                {
                    Conntect();
                }
            };
            _client = new MqttFactory().CreateMqttClient();
            _client.ConnectingAsync += _ =>
            {
                ClientController.WriteLog("Mqtt", "Connecting");
                return Task.CompletedTask;
            };
            _client.ConnectedAsync += _ =>
            {
                _timer.Interval = config.HeartBeatSecond * 1000;
                ClientController.WriteLog("Mqtt", _.ConnectResult.ResultCode.ToString());
                Subscribe();
                ClientController.HeartBeat(new ClientToServer.HeartBeat(_code));
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
                _timer.Interval = 3000;
                return Task.CompletedTask;
            };
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithWebSocketServer(config.Server)
                .WithCredentials(config.MqttUser, config.MqttPassword);
            ClientController.HeartBeat = o => 
            Send(ClientToServer.TopicTypeEnum.heartbeat, JsonSerializer.Serialize(o));
            ClientController.MaterialDownloadStatus = o => 
            Send(ClientToServer.TopicTypeEnum.material_download_status, JsonSerializer.Serialize(o));
            ClientController.WriteLog = WrietLog;
            _timer.Start();
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
            _timer.Close();
            _client.Dispose();
        }
        private async void Subscribe()
        {
            foreach (var topic in Enum.GetNames(typeof(ServerToClient.TopicTypeEnum)))
            {
                await _client.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
            }
        }
        private static void Receive(ServerToClient.TopicTypeEnum topic, string json)
        {
            ClientController.WriteLog(topic.ToString(), json);
            switch (topic)
            {
                case ServerToClient.TopicTypeEnum.delete_material:
                    if (TryFromJson<DeleteMaterial>(json, out var deleteMaterial))
                        ClientController.DeleteMaterial(deleteMaterial!);
                    break;
                case ServerToClient.TopicTypeEnum.delete_new_flash_content:
                    if (TryFromJson<DeleteNewFlashContent>(json, out var deleteNewFlashContent))
                        ClientController.DeleteNewFlashContent(deleteNewFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.material_download_url:
                    if (TryFromJson<MaterialDownloadUrl>(json, out var materialDownloadUrl))
                        ClientController.MaterialDownloadUrl(materialDownloadUrl!);
                    break;
                case ServerToClient.TopicTypeEnum.new_flash_content:
                    if (TryFromJson<BaseContent>(json, out var newFlashContent))
                        ClientController.NewFlashContent(newFlashContent!);
                    break;
                case ServerToClient.TopicTypeEnum.content:
                    if (TryFromJson<BaseContent>(json, out var normalContent))
                        ClientController.NormalContent(normalContent!);
                    break;
                case ServerToClient.TopicTypeEnum.emergency_content:
                    if (TryFromJson<BaseContent>(json, out var emergencyContent))
                        ClientController.EmergrgencyContent(emergencyContent!);
                    break;
                case ServerToClient.TopicTypeEnum.timing_boot:
                    if (TryFromJson<TimingBoot>(json, out var timingBoot))
                        ClientController.TimingBoot(timingBoot!);
                    break;
                case ServerToClient.TopicTypeEnum.timing_volume:
                    if (TryFromJson<TimingVolume>(json, out var timingVolume))
                        ClientController.TimingVolume(timingVolume!);
                    break;
            }
        }
        private async void Send(ClientToServer.TopicTypeEnum topic, string json)
        {
            await _client.PublishStringAsync(topic.ToString(), json);
            ClientController.WriteLog(topic.ToString(), json);
        }
        private static void WrietLog(string title, string content)
        {
            if (!Directory.Exists("./log/")) Directory.CreateDirectory("./log/");
            File.WriteAllText($"./log/{title}-{DateTime.Now.Ticks}.json", content);
            Console.WriteLine($"{DateTime.Now}\t[{title}]\t{content}");
        }
        private static bool TryFromJson<T>(string json,out T? result)
        {
            result = JsonSerializer.Deserialize<T>(json);
            return result is not null;
        }
        ~ClientConnect()
        {
            Close();
        }
    }
}
