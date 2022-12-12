using System;
using System.Collections.Generic;
using System.Text;

namespace ClientCore.Controllers
{
    internal partial class ClientController
    {
        private void DeleteMaterial(ServerToClient.DeleteMaterial deleteMaterial)
        {
            if (deleteMaterial.Materials != null)
                foreach (var material in deleteMaterial.Materials)
                    if (material.Content != null)
                    {
                        var uri = new Uri(material.Content);
                        var id = material.Id;
                        var ext = System.IO.Path.GetExtension(uri.Segments[^1]);
                        var file = System.IO.Path.Combine(_clientConfig.MaterialPath, $"{id}{ext}");
                        _client.DeleteFiles(new string[] { file });
                    }
            if (deleteMaterial.DeleteAll)
            {
                var files = System.IO.Directory.GetFiles(_clientConfig.MaterialPath);
                _client.DeleteFiles(files);
            }
        }
        private void MaterialDownloadUrl(ServerToClient.MaterialDownloadUrl material)
        {
            var id = material.ContentId;
            var deviceId = material.DeviceId;
            var deviceGroupId = material.DeviceGroupId;
            if (!string.IsNullOrEmpty(material.Url))
            {
                var url = material.Url;
            }
        }
    }
}
