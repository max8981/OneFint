using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using System.Threading.Tasks;

namespace 屏幕管理
{
    public class Mqtt
    {
        private IMqttClient _mqttClient;
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        private readonly MqttClientOptions _options;
        internal Action<ServerToClient.TopicTypeEnum, string> Receive = (t, j) => { };
        internal Action Connected = () => { };
        internal Mqtt(string server,int port,string user="admin",string password= "public")
        {
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer(server, port)
                .WithCredentials(user,password);
            _options = _optionsBuilder.Build();
            _mqttClient = MqttClientInit();
        }
        private IMqttClient MqttClientInit()
        {
            var result = new MqttFactory().CreateMqttClient();
            result.ConnectingAsync += _ =>
            {
                Global.ShowMessage($"正在连接服务器", 3);
                Log.Default.Info(_.ClientOptions.WillContentType, "ConnectingAsync");
                return Task.CompletedTask;
            };
            result.ConnectedAsync += _ =>
            {
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
                Log.Default.Error(ex, "ConnectAsync");
                Global.ShowMessage($"连接服务器失败：{ex.Message}", 5);
            }
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
