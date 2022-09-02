// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client;
using System.Text;

Console.WriteLine("Hello, World!");
var factory = new MqttFactory();
var options =new MqttClientOptionsBuilder()
    .WithClientId("clientId_max")
    .WithWebSocketServer("ws://47.101.178.160:8083/ws")
    .WithCredentials("admin","public")
    .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
    .Build();
MqttClient mqttClient = factory.CreateMqttClient() as MqttClient;
mqttClient.ConnectingAsync += a => { return new Task(() => Console.WriteLine(a.ToString())); };
mqttClient.ConnectedAsync += a => { return new Task(() => Console.WriteLine(a.ToString())); };
try
{
    await mqttClient.ConnectAsync(options);
    Console.WriteLine("正在连接");
    await mqttClient.SubscribeAsync("heartbeat", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
while (true)
{
    Console.ReadKey();
};
