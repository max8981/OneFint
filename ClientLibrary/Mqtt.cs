﻿using ClientLibrary.Models;
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
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private readonly MqttClientOptions _options;
        private readonly IClient _client;
        internal Action<TopicTypeEnum, string> Receive = (t, j) => { };
        internal Action Connected = () => { };
        internal Mqtt(IClient client)
        {
            _client = client;
            Downloader.SetMaterialsPath(client.Config.MaterialPath);
            _mqttClient = new MqttFactory().CreateMqttClient();
            _mqttClient.ConnectingAsync += _ =>
            {
                _client.WriteLog("Mqtt-Connecting", $"{_.ClientOptions.WillContentType}");
                _client.ShowMessage($"正在连接服务器", new TimeSpan(0, 0, 3));
                return Task.CompletedTask;
            };
            _mqttClient.ConnectedAsync += _ =>
            {
                _client.WriteLog("Mqtt-Connected", _.ConnectResult.ResultCode.ToString());
                Subscribe();
                _client.ShowMessage($"服务器连接成功", new TimeSpan(0, 0, 5));
                Connected();
                return Task.CompletedTask;
            };
            _mqttClient.ApplicationMessageReceivedAsync += arg =>
            {
                var topic = (ServerToClient.TopicTypeEnum)Enum.Parse(typeof(ServerToClient.TopicTypeEnum), arg.ApplicationMessage.Topic);
                var json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                Receive(topic, json);
                return Task.CompletedTask;
            };
            _mqttClient.DisconnectedAsync += _ =>
            {
                _client.WriteLog("Mqtt-Disconnected", _.Reason.ToString());
                _client.ShowMessage($"网络已断开", new TimeSpan(0, 0, 5));
                return Task.CompletedTask;
            };
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_client.Config.MqttServer,_client.Config.MqttPort)
                //.WithWebSocketServer(_client.Config.Server)
                .WithCredentials(_client.Config.MqttUser, _client.Config.MqttPassword);
            _options = _optionsBuilder.Build();
        }
        internal async void Connect()
        {
            try
            {
                await _mqttClient.ConnectAsync(_options);
            }
            catch (Exception ex)
            {
                _client.WriteLog($"Mqtt-Connect-Exception", ex.Message);
                _client.ShowMessage($"连接服务器失败：{ex.Message}",new TimeSpan(0,0,5));
            }
        }
        internal async void Send(ClientToServer.TopicTypeEnum topic, string json)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.PublishStringAsync(topic.ToString(), json);
                _client.WriteLog($"Mqtt-Send-{topic}", $"{json}");
            }
            else
            {
                await _mqttClient.DisconnectAsync();
                Connect();
            }

        }
        internal void Close()
        {
            _mqttClient.Dispose();
        }
        private async void Subscribe()
        {
            foreach (var topic in Enum.GetNames(typeof(ServerToClient.TopicTypeEnum)))
            {
                await _mqttClient.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
            }
        }
        ~Mqtt()
        {
            Close();
        }
    }
}