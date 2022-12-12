using Client;
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
using System.Threading.Tasks;

namespace Clietn
{
    public class Mqtt
    {
        private IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private readonly MqttClientOptions _options;
        private readonly ClientConfig _config;
        internal Action<string, string> Receive = (t, j) => { };
        internal Action Connected = () => { };
        internal Mqtt(ClientConfig client)
        {
            _config = client;
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.MqttServer,_config.MqttPort)
                .WithCredentials(_config.MqttUser, _config.MqttPassword);
            _options = _optionsBuilder.Build();
            _mqttClient = MqttClientInit();
        }
        private IMqttClient MqttClientInit()
        {
            var result = new MqttFactory().CreateMqttClient();
            result.ConnectingAsync += _ =>
            {
                ClientCore.Log.Default.Info(_.ClientOptions.WillContentType, "ConnectingAsync");
                return Task.CompletedTask;
            };
            result.ConnectedAsync += _ =>
            {
                ClientCore.Log.Default.Info(_.ConnectResult.ResultCode.ToString(), "ConnectedAsync");
                SubscribeAsync();
                Connected();
                return Task.CompletedTask;
            };
            result.ApplicationMessageReceivedAsync += arg =>
            {
                var json = string.Empty;
                try
                {
                    var topic = arg.ApplicationMessage.Topic;
                    json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                    Receive(topic, json);
                }
                catch (Exception ex)
                {
                    ClientCore.Log.Default.Error(ex, "ApplicationMessageReceivedAsync", arg.ApplicationMessage.Topic, json);
                }
                return Task.CompletedTask;
            };
            result.DisconnectedAsync += _ =>
            {
                ClientCore.Log.Default.Warn(_.Reason.ToString(), "DisconnectedAsync");
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
                ClientCore.Log.Default.Error(ex, "ConnectAsync");
            }
        }
        internal async void Send(string topic, string json)
        {
            try
            {
                if (_mqttClient.IsConnected)
                {
                    await _mqttClient.PublishStringAsync(topic, json);
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
                ClientCore.Log.Default.Error(ex, "PublishStringAsync", $"{nameof(_mqttClient.IsConnected)}:{_mqttClient.IsConnected}", topic.ToString(), json);
            }
        }
        internal void Close()
        {
            _mqttClient.Dispose();
        }
        private async void SubscribeAsync()
        {
            foreach (var topic in Enum.GetNames(typeof(ClientCore.ServerToClient.TopicTypeEnum)))
            {
                try
                {
                    await _mqttClient.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
                }
                catch(Exception ex)
                {
                    ClientCore.Log.Default.Error(ex, "SubscribeAsync", topic);
                }
            }
        }
        ~Mqtt()
        {
            Close();
        }
    }
}
