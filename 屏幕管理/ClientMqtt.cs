using Microsoft.VisualBasic.ApplicationServices;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using 屏幕管理.Systems;

namespace 屏幕管理
{
    public class ClientMqtt
    {
        private IMqttClient _mqttClient;
        private MqttClientOptionsBuilder _optionsBuilder;
        private MqttClientOptions _options;
        internal Action<ServerToClient.TopicTypeEnum, string> Receive = (t, j) => { };
        internal Action Connected = () => { };
        internal ClientMqtt(string server, int port, string user = "admin", string password = "public")
        {
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithCredentials(user, password);
            _options = _optionsBuilder.Build();
            _mqttClient = MqttClientInit();
        }
        private IMqttClient MqttClientInit()
        {
            var result = new MqttFactory().CreateMqttClient();
            result.ConnectingAsync += _ =>
            {
                Global.MQTTLog("Mqtt-Connecting", $"{_.ClientOptions.ChannelOptions}");
                Global.ShowMessage($"正在连接服务器", 3);
                Log.Default.Info(_.ClientOptions.WillContentType, "ConnectingAsync");
                return Task.CompletedTask;
            };
            result.ConnectedAsync += _ =>
            {
                Global.MQTTLog("Mqtt-Connected", _.ConnectResult.ResultCode.ToString());
                Log.Default.Info(_.ConnectResult.ResultCode.ToString(), "ConnectedAsync");
                SubscribeAsync();
                Global.ShowMessage($"服务器连接成功", 5);
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
                    Global.MQTTLog(arg.ApplicationMessage.Topic, json);
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
                Global.MQTTLog("Mqtt-Disconnected", _.Reason.ToString());
                Log.Default.Warn(_.Reason.ToString(), "DisconnectedAsync");
                Global.ShowMessage($"网络已断开", 5);
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
                Global.MQTTLog($"Mqtt-Connect-Exception", ex.Message);
                Log.Default.Error(ex, "ConnectAsync");
                Global.ShowMessage($"连接服务器失败：{ex.Message}", 5);
            }
        }
        internal async ValueTask<bool> ReConnectAsync(string server,int port, string user = "admin", string password = "public")
        {
            var result = false;
            await _mqttClient.DisconnectAsync();
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithCredentials(user, password);
            _options = _optionsBuilder.Build();
            try
            {
                await _mqttClient.ConnectAsync(_options);
                result = true;
            }
            catch (Exception ex)
            {
                Global.MQTTLog($"Mqtt-Connect-Exception", ex.Message);
                Log.Default.Error(ex, "ConnectAsync");
                Global.ShowMessage($"连接服务器失败：{ex.Message}", 5);
            }
            return result;
        }
        internal async void Send(ClientToServer.TopicTypeEnum topic, string json)
        {
            try
            {
                if (_mqttClient.IsConnected)
                {
                    await _mqttClient.PublishStringAsync(topic.ToString(), json);
                    Global.MQTTLog($"Mqtt-Send-{topic}", $"{json}");
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
                Global.MQTTLog($"Mqtt-Send", $"{ex.Message}");
            }
        }
        internal void Close()
        {
            _mqttClient.DisconnectAsync().Wait();
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
                    Global.MQTTLog("SubscribeAsync-Exception", topic);
                    Log.Default.Error(ex, "SubscribeAsync", topic);
                }
            }
        }
        ~ClientMqtt()
        {
            _ = _mqttClient.DisconnectAsync();
            _mqttClient.Dispose();
            Close();
        }
    }
}
