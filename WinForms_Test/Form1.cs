using MQTTnet.Client;
using MQTTnet;
using System.Text.Json;
using System.Text.Unicode;

namespace WinForms_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            var options = new MqttClientOptionsBuilder()
                .WithClientId("clientId_max")
                .WithWebSocketServer("ws://47.101.178.160:8083/mqtt")
                .WithCredentials("admin", "public")
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                .Build();
            var mqttClient = new MqttFactory().CreateMqttClient();
            mqttClient.ConnectingAsync += ConnectingHandlerAsync;
            mqttClient.ConnectedAsync += ConnectedHandlerAsync;
            mqttClient.ApplicationMessageReceivedAsync += MessageReceivedHandlerAsync;
            try
            {
                await mqttClient.ConnectAsync(options);

                await mqttClient.SubscribeAsync("timing_volume", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                Heartbeat(mqttClient);
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
                button1.Enabled=false;
            }
        }
        private void AppendMessage(string message)
        {
            Console.WriteLine(message);
            Invoke(() =>
            {
                richTextBox1.AppendText($"{message}\r\n");
            });
        }
        private Task ConnectingHandlerAsync(MqttClientConnectingEventArgs eventArgs)
        {
            AppendMessage("正在连接");
            return Task.CompletedTask;
        }
        private Task ConnectedHandlerAsync(MqttClientConnectedEventArgs eventArgs)
        {
            AppendMessage("连接成功");
            return Task.CompletedTask;
        }
        private Task MessageReceivedHandlerAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            var payload = eventArgs.ApplicationMessage.Payload;
            AppendMessage(System.Text.Encoding.UTF8.GetString(payload));
            return Task.CompletedTask;
        }
        private async void Heartbeat(IMqttClient client)
        {
            JsonSerializerOptions options = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            var json = JsonSerializer.Serialize(new Topic.Heartbeat { Code = "max01" });
            var topic = "/heartbeat/";
            while (true)
            {
                await client.PublishStringAsync(topic, json);
                await Task.Delay(5000);
            }
        }
    }
}