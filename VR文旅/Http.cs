using CefSharp.DevTools.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace VR文旅
{
    internal static class Http
    {
        private static readonly HttpClient _client = new();
        private static readonly Dictionary<FileInfo, bool> _downloadTasks = new();
        private static readonly JsonSerializerOptions options = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        public static async Task<T> PostAsync<T>(this object request,string path)where T : IResponse, new()
        {
            var json=JsonSerializer.Serialize(request,options);
            return await PostAsync<T>(path, json);
        }
        public static async Task<string> GetAsync(string url)
        {
            var result = string.Empty;
            try
            {
                result = await _client.GetStringAsync(url);
            }
            catch(Exception ex)
            {
                Log.Default.Error(ex, "PostAsync", url);
            }
            return result;
        }
        public static async Task<bool> DownloadAsync(this FileInfo file, string? url)
        {
            var result = file.Exists;
            if (!result&& !string.IsNullOrEmpty(url))
            {
                if (_downloadTasks.TryAdd(file, false))
                {
                    var tempFile = new FileInfo(Path.GetTempFileName());
                    try
                    {
                        var response = await _client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            using var stream = await response.Content.ReadAsStreamAsync();
                            using var fileStream = tempFile.Create();
                            //using var fileStream = file.Create();
                            await stream.CopyToAsync(fileStream);
                            if (!Directory.Exists(file.DirectoryName))
                                _ = Directory.CreateDirectory(file.DirectoryName ?? "./");
                            SpinWait.SpinUntil(() =>
                            {
                                fileStream.Dispose();
                                try
                                {
                                    //tempFile.MoveTo(file.FullName);
                                    System.IO.File.Move(tempFile.FullName, file.FullName, true);
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            });
                            result = true;
                            _downloadTasks[file] = true;
                        }
                    }
                    catch
                    {

                    }
                }
                else
                    result = _downloadTasks[file];
            }
            return result;
        }
        public static bool GetDownloadStatus(this FileInfo file, string? url)
        {
            Task.Factory.StartNew(async () => await file.DownloadAsync(url));
            return file.Exists;
        }
        private static async ValueTask<T> PostAsync<T>(string path, string json) where T : IResponse, new()
        {
            var url = CombineUrl(Systems.Config.Server, path);
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
