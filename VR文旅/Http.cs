using CefSharp.DevTools.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace VR文旅
{
    internal static class Http
    {
        private static readonly HttpClient _client = new();
        public static string Host { get; private set; } = "http://47.101.178.160:8079";
        public static async Task<T> PostAsync<T>(this object request,string path)where T : IResponse, new()
        {
            var json=JsonSerializer.Serialize(request,new JsonSerializerOptions(JsonSerializerDefaults.General));
            return await PostAsync<T>(path, json);
        }
        private static async ValueTask<T> PostAsync<T>(string path, string json) where T : IResponse, new()
        {
            var url = CombineUrl(Host, path);
            var content = new StringContent(json);
            T result = new();
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            try
            {
                var response = await _client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responsejson = await response.Content.ReadAsStringAsync();
                    var obj = JsonSerializer.Deserialize<T>(responsejson);
                    if (obj is not null)
                    {
                        result = obj;
                        result.Success = true;
                    }
                }
                else
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    Log.Default.Warn(response.StatusCode.ToString(), "PostAsync", url, json, contentString);
                }
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "PostAsync", url, json);
            }
            return result;
        }
        private static string CombineUrl(string s1, string s2)
        {
            s1 = s1.TrimEnd('/');
            s2 = s2.TrimStart('/');
            return $"{s1}/{s2}";
        }
        public interface IResponse
        {
            bool Success { get; set; }
        }
    }
}
