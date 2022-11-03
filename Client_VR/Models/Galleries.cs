using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Client_VR.Models
{
    internal class Galleries
    {
        private const string MEMBER = "https://apiv4.720yun.com/member/me";
        private const string HOST = "https://api-oms.720yun.com/web/content/data";
        private static HttpClient _client = new()/* { BaseAddress=new Uri(HOST)}*/;
        private static JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        public static async Task<Galler[]> GetGalleriesAsync()
        {
            var result = new List<Galler>();
            int page = 1;
            HttpRequestMessage httpRequest = new(HttpMethod.Options, MEMBER);
            var op = await _client.SendAsync(httpRequest);
            httpRequest = new(HttpMethod.Options, HOST + $"?channelId=44&subChannelId=0&page={page}&pageSize=60&sort=0");
            var get = await _client.SendAsync(httpRequest);
            HttpRequestMessage getRequest = new(HttpMethod.Get, MEMBER);
            var op2 = await _client.SendAsync(getRequest);
            getRequest = new(HttpMethod.Get, HOST + $"?channelId=44&subChannelId=0&page={page}&pageSize=60&sort=0");
            var get2 = await _client.SendAsync(getRequest);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/html"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:105.0) Gecko/20100101 Firefox/105.0");
            _client.DefaultRequestHeaders.Host = "api-oms.720yun.com";

            while (true)
            {
                var query = $"?channelId=44&subChannelId=0&page={page}&pageSize=60&sort=0";
                var a = await _client.GetAsync(query);
                var response = await a.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(response))
                {
                    var datas = JsonSerializer.Deserialize<DataResponse>(response, options);
                    if(datas?.Data?.List.Length > 0)
                    {
                        result.AddRange(datas.Data.List);
                        page++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            }
            return result.ToArray();
        }
        public class DataResponse
        {
            public int Success { get; set; }
            public string? Message { get; set; }
            public ListData? Data { get; set; }
            public class ListData
            {
                public int Count { get; set; }
                public Galler[] List { get; set; } = Array.Empty<Galler>();
            }
        }
    }
}
