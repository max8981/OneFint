using Downloader;
using DownloadQueue;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using 屏幕管理.Systems;

namespace 屏幕管理.Controllers
{
    internal class MaterialController
    {
        private static readonly DownloadConfiguration _configuration = new()
        {
            BufferBlockSize = 1024,
            ChunkCount = 5,
            MaximumBytesPerSecond = 0,
            MaxTryAgainOnFailover = int.MaxValue,
            ParallelDownload = true,
            ParallelCount = 5,
            Timeout = 5000,
        };
        private static readonly ConcurrentDictionary<string, MaterialState> _materials = Load();
        private static readonly string materialPath = Path.Combine(DownloadPath, "materials");
        private static readonly DirectoryInfo tempPath = new(Path.Combine(DownloadPath, "temp"));
        private static string MaterialsFile => Path.Combine(materialPath, "materials.json");
        public MaterialController() {

        }
        public static string DownloadPath { get; set; } = AppContext.BaseDirectory;
        public static int DownloadTaskCount { get; set; } = 3;
        public bool TryGetMaterial(Models.Material material,out string fileName)
        {
            var result = false;
            fileName = "";
            var id = material.Id;
            var content = material.Content;
            if (!string.IsNullOrEmpty(content))
            {
                var uri = new Uri(content);
                var ext = Path.GetExtension(uri.Segments[^1]);
                fileName = Path.Combine(materialPath, $"{id}{ext}");
                if (_materials.TryGetValue(content, out var value))
                {
                    var state = value;
                    if (!value.IsSaveComplete)
                    {
                        try
                        {
                            File.Move(value.FileName, fileName);
                            state.IsSaveComplete = true;
                        }
                        catch
                        {

                        }
                    }
                    state.IsReport = state.IsReport || Report(id);
                    _materials.TryUpdate(content, state, value);
                }
                else
                {
                    Download(content);
                }
            }
            return result;
        }
        public static void MaterialDownload(ServerToClient.MaterialDownloadUrl materialDownloadUrl)
        {
            var url = materialDownloadUrl.Url;
            if (!string.IsNullOrEmpty(url))
                Download(url);
        }
        private static string GetUrlFileName(Uri uri)=> System.Web.HttpUtility.UrlDecode(uri.Segments.Last());
        private static async void Download(string url)
        {
            var state = new MaterialState()
            {
                Url= url,
            };
            if(_materials.TryAdd(url, state))
            {
                using DownloadService download = new(_configuration);
                download.DownloadStarted += (o, e) =>
                {
                    if (o is DownloadService download)
                    {
                        if(_materials.TryGetValue(url, out var value))
                        {
                            state.FileName = e.FileName;
                            state.Package = download.Package;
                            state.IsDownloading = true;
                            _materials.TryUpdate(url, state, value);
                        }
                    }
                };
                download.DownloadFileCompleted += (o, e) =>
                {
                    if (o is DownloadService download)
                    {
                        state.IsDownloading = false;
                        if (_materials.TryGetValue(url, out var value))
                            _materials.TryUpdate(url, state, value);
                    }
                };
                await download.DownloadFileTaskAsync(url, tempPath);
            }
        }
        private static async void Save()
        {
            var json = JsonSerializer.Serialize(_materials);
            await File.WriteAllTextAsync(json, MaterialsFile);
        }
        private static ConcurrentDictionary<string, MaterialState> Load()
        {
            ConcurrentDictionary<string, MaterialState> result;
            if(!Directory.Exists(materialPath))
                Directory.CreateDirectory(materialPath);
            if(tempPath.Exists)
                tempPath.Delete();
            if (File.Exists(MaterialsFile))
            {
                try
                {
                    var json = File.ReadAllText(MaterialsFile);
                    result = JsonSerializer.Deserialize<ConcurrentDictionary<string, MaterialState>>(json) ?? new();
                    foreach (var item in result)
                    {
                        if (!File.Exists(item.Value.FileName))
                        {
                            var state=item.Value;
                            state.IsDownloading=false;
                            state.IsSaveComplete=false;
                            state.IsReport = false;
                            state.FileName = "";
                            result.TryUpdate(item.Key, state, item.Value);
                        }
                    }
                }
                catch
                {
                    File.Delete(MaterialsFile);
                    return new();
                }
            }
            else
            {
                result = new();
            }
            return result;
        }
        private bool Report(int materialId)
        {
            int? deviceId = Config.DeviceId > 0 ? Config.DeviceId : null;
            int? deviceGroupId=Config.DeviceGroupId > 0 ? Config.DeviceGroupId : null;
            var status = new ClientToServer.MaterialDownloadStatus(materialId, true, deviceId, deviceGroupId);
            //_mqtt.Send(ClientToServer.TopicTypeEnum.material_download_status, JsonSerializer.Serialize(status));
            return false;
        }
        struct MaterialState
        {
            public string Url { get; set; }
            public string FileName { get; set; }
            public bool IsDownloading { get; set; }
            public bool IsSaveComplete { get; set; }
            public bool IsReport { get; set; }
            public DownloadPackage Package { get; set; }
        }
    }
}
