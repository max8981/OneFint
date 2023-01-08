using System;
using System.Collections.Generic;
using System.Text;

namespace 屏幕管理.Controllers
{
    internal partial class ClientController
    {
        private void DeleteMaterial(ServerToClient.DeleteMaterial deleteMaterial)
        {
            if (deleteMaterial.Materials != null)
                foreach (var material in deleteMaterial.Materials)
                    if (DownloadController.TryGetMaterialFilePath(material, out var file))
                        System.IO.File.Delete(file);
            if (deleteMaterial.DeleteAll)
                foreach (var file in DownloadController.MaterialFiles)
                    System.IO.File.Delete(file);
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
