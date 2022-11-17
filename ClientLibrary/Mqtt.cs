using ClientLibrary.Models;
using ClientLibrary.ServerToClient;
using ClientLibrary.UIs;
using Microsoft.VisualBasic;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ClientLibrary
{
    public class Mqtt
    {
        private IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private readonly MqttClientOptions _options;
        private readonly IClient _client;
        internal Action<TopicTypeEnum, string> Receive = (t, j) => { };
        internal Action Connected = () => { };
        internal Mqtt(IClient client)
        {
            _client = client;
            Downloader.SetMaterialsPath(client.Config.MaterialPath);
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_client.Config.MqttServer,_client.Config.MqttPort)
                .WithCredentials(_client.Config.MqttUser, _client.Config.MqttPassword);
            _options = _optionsBuilder.Build();
            _mqttClient = MqttClientInit();
        }
        private IMqttClient MqttClientInit()
        {
            var result = new MqttFactory().CreateMqttClient();
            result.ConnectingAsync += _ =>
            {
                _client.WriteLog("Mqtt-Connecting", $"{_.ClientOptions.WillContentType}");
                _client.ShowMessage($"正在连接服务器", new TimeSpan(0, 0, 3));
                Log.Default.Info(_.ClientOptions.WillContentType, "ConnectingAsync");
                return Task.CompletedTask;
            };
            result.ConnectedAsync += _ =>
            {
                _client.WriteLog("Mqtt-Connected", _.ConnectResult.ResultCode.ToString());
                Log.Default.Info(_.ConnectResult.ResultCode.ToString(), "ConnectedAsync");
                SubscribeAsync();
                _client.ShowMessage($"服务器连接成功", new TimeSpan(0, 0, 5));
                Connected();
                return Task.CompletedTask;
            };
            result.ApplicationMessageReceivedAsync += arg =>
            {
                var json = string.Empty;
                try
                {
                    var topic = (ServerToClient.TopicTypeEnum)Enum.Parse(typeof(ServerToClient.TopicTypeEnum), arg.ApplicationMessage.Topic);
                    json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                    Receive(topic, json);
                }
                catch (Exception ex)
                {
                    Log.Default.Error(ex, "ApplicationMessageReceivedAsync", arg.ApplicationMessage.Topic, json);
                }
                return Task.CompletedTask;
            };
            result.DisconnectedAsync += _ =>
            {
                Log.Default.Warn(_.Reason.ToString(), "DisconnectedAsync");
                _client.WriteLog("Mqtt-Disconnected", _.Reason.ToString());
                _client.ShowMessage($"网络已断开", new TimeSpan(0, 0, 5));
                return Task.CompletedTask;
            };
            return result;
        }
        internal async void ConnectAsync()
        {
            try
            {
                await _mqttClient.ConnectAsync(_options);
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "ConnectAsync");
                _client.WriteLog($"Mqtt-Connect-Exception", ex.Message);
                _client.ShowMessage($"连接服务器失败：{ex.Message}",new TimeSpan(0,0,5));
            }
        }
        internal async void Send(ClientToServer.TopicTypeEnum topic, string json)
        {
            try
            {
                if (_mqttClient.IsConnected)
                {
                    await _mqttClient.PublishStringAsync(topic.ToString(), json);
                    _client.WriteLog($"Mqtt-Send-{topic}", $"{json}");
                }
                else
                {
                    await _mqttClient.DisconnectAsync();
                    _mqttClient = MqttClientInit();
                    ConnectAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex, "PublishStringAsync", $"{nameof(_mqttClient.IsConnected)}:{_mqttClient.IsConnected}", topic.ToString(), json);
                _client.WriteLog($"Mqtt-Send", $"{ex.Message}");
            }
        }
        internal void Close()
        {
            _mqttClient.Dispose();
        }
        private async void SubscribeAsync()
        {
            foreach (var topic in Enum.GetNames(typeof(ServerToClient.TopicTypeEnum)))
            {
                try
                {
                    await _mqttClient.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
                }
                catch(Exception ex)
                {
                    Log.Default.Error(ex, "SubscribeAsync", topic);
                }
            }
        }
        ~Mqtt()
        {
            Close();
        }
    }
}
