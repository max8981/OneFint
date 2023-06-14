using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CmsrLogin
{
    internal class NetCheck
    {
        private readonly System.Net.NetworkInformation.Ping _ping = new System.Net.NetworkInformation.Ping();
        private readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient();
        public bool Dns(string host)
        {
            try
            {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(host);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public bool Ping(string host, int timeout = 1000)
        {
            System.Net.NetworkInformation.PingReply reply = _ping.Send(host, timeout);
            Console.WriteLine($"ping {reply.Address} {reply.Status} {reply.RoundtripTime} ");
            return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
        }
        public async Task<bool> CheckTime()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://quan.suning.com/getSysTime.do");
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    System.Text.Json.Nodes.JsonNode? jsonNode = System.Text.Json.Nodes.JsonNode.Parse(responseString);
                    if (jsonNode != null)
                    {
                        System.Text.Json.Nodes.JsonObject jsonObject = jsonNode.AsObject();
                        var t1 = jsonObject["sysTime1"];
                        var t2 = jsonObject["sysTime2"];
                        if (DateTime.TryParse(t2?.GetValue<string>(), out var time))
                        {
                            Console.WriteLine($"Time: {time} Checked");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public async Task<bool> CmsrLogin()
        {
            var url = "http://192.168.210.200/ac_portal/login.php";
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            var content = new StringContent("opr=pwdLogin&userName=yuwei&pwd=7c081e321d85&auth_tag=1678757300642&rememberPwd=0", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                System.Text.Json.Nodes.JsonNode? jsonNode = System.Text.Json.Nodes.JsonNode.Parse(responseString);
                if (jsonNode != null)
                {
                    System.Text.Json.Nodes.JsonObject jsonObject = jsonNode.AsObject();
                    var success = jsonObject["success"]?.GetValue<bool>();
                    var msg = jsonObject["msg"]?.GetValue<string>();
                    Console.WriteLine(msg);
                    return success ?? false;
                }
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
            }
            return false;
        }
        public async Task<bool> CmsrLogout()
        {
            var url = "http://192.168.210.200/ac_portal/login.php";
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
            var content = new StringContent("opr=logout", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _httpClient.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                System.Text.Json.Nodes.JsonNode? jsonNode = System.Text.Json.Nodes.JsonNode.Parse(responseString);
                if (jsonNode != null)
                {
                    System.Text.Json.Nodes.JsonObject jsonObject = jsonNode.AsObject();
                    var success = jsonObject["success"]?.GetValue<bool>();
                    var msg = jsonObject["msg"]?.GetValue<string>();
                    Console.WriteLine(msg);
                    return success ?? false;
                }
                Console.WriteLine(responseString);
            }
            else
            {
                Console.WriteLine(response.StatusCode.ToString());
            }
            return false;
        }
    }
}
