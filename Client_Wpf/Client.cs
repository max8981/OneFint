using MQTTnet.Client;
using MQTTnet;
using SharedProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharedProject.IClientControl;
using System.Timers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Masuit.Tools.Net;
using System.IO;

namespace Client_Wpf
{
    internal class Client:IClientControl
    {
        private readonly string _code;
        private bool _connected = false;
        private readonly Timer _timer = new Timer(10 * 1000)
        {
            AutoReset = true,
        };
        private readonly IMqttClient _client = new MqttFactory().CreateMqttClient();
        //private readonly HttpClient _httpClient = new HttpClient();
        private readonly MqttClientOptionsBuilder _optionsBuilder;
        public delegate void NormalContentHandler(NormalContent content);
        public event NormalContentHandler NormalContentEvent;
        public Action<string, string> Command;
        public Action<string, string, int> LogWrite;
        public Action<NormalContent> Normal;
        public Action<EmergencyContent> Emergency;
        public Action<NewFlashContent> NewFlash;
        public Action<CustomControls.DownloadProgressControl.DownloadTask, Action<CustomControls.DownloadProgressControl.DownloadTask>> Downloader;
        public Client(string server, string code, string user = "admin", string pwd = "public")
        {
            _code = code;
            _timer.Elapsed += (o, e) =>
            {
                if (_connected)
                {
                    HeartBeat();
                }
                else
                {
                    Conntect();
                }
            };
            _client.ConnectingAsync += _ => {
                Console.WriteLine($"{DateTime.Now}[Connect]{_.ClientOptions.ChannelOptions}");
                return Task.CompletedTask;
            };
            _client.ConnectedAsync += _ => {
                Console.WriteLine($"{DateTime.Now}[Connect]{_.ConnectResult.ResultCode}");
                Subscribe();
                HeartBeat();
                _connected = true;
                return Task.CompletedTask;
            };
            _client.ApplicationMessageReceivedAsync += arg => {
                var topic = (IClientControl.TopicEnum)Enum.Parse(typeof(IClientControl.TopicEnum), arg.ApplicationMessage.Topic);
                var json = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
                Receive(topic, json);
                return Task.CompletedTask;
            };
            _client.DisconnectedAsync += _ => {
                Console.WriteLine($"{DateTime.Now}[Connect]{_.Reason}");
                _connected = false;
                _client.Dispose();
                return Task.CompletedTask;
            };
            _optionsBuilder = new MqttClientOptionsBuilder()
                .WithWebSocketServer(server)
                .WithCredentials(user, pwd);
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
                Console.WriteLine(ex.Message);
            }
        }
        public async void Subscribe()
        {
            foreach (var topic in Enum.GetNames(typeof(TopicEnum)))
            {
                await _client.SubscribeAsync(topic, MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce);
            }
        }
        public void Close()
        {
            _timer.Stop();
            _client.Dispose();
        }
        public void HeartBeat()
        {
            Send("heartbeat", JsonSerializer.Serialize(new { code = _code }));
        }
        public void SendDownloadStatus(int contentId, int? deviceId,int? deviceGroupId)
        {
            Send("material_download_status", JsonSerializer.Serialize(new
            {
                device_id = deviceId,
                content_id = contentId,
                downloaded = true,
                device_group_id = deviceGroupId,
            }));
        }

        public void Receive(TopicEnum topic, string json)
        {
            System.IO.File.WriteAllText($"./log/{topic}-{DateTime.Now.Ticks}.json", json);
            switch (topic)
            {
                case TopicEnum.content:
                    NormalContentEvent(NormalContent.FromJson(json));
                    break;
                case TopicEnum.emergency_content:
                    Emergency(EmergencyContent.FromJson(json));break;
                case TopicEnum.new_flash_content:
                    NewFlash(NewFlashContent.FromJson(json));break;
                case TopicEnum.material_download_url:
                    MaterialDownload(MaterialDownloadUrl.FromJson(json)); break;
                default: break;
            }
        }

        public async void Send(string topic, string json)
        {
            await _client.PublishStringAsync(topic, json);
        }
        private void MaterialDownload(MaterialDownloadUrl material)
        {
            Downloader(
            new CustomControls.DownloadProgressControl.DownloadTask(material.ContentId,material.Url),
            a =>
            {
                Send("material_download_status", JsonSerializer.Serialize(new
                {
                    device_id = material.DeviceId,
                    content_id = material.ContentId,
                    downloaded = true,
                    device_group_id = material.DeviceGroupId,
                }));
            });
        }

        ~Client()
        {
            Close();
        }
    }
}
