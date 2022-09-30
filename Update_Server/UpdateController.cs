using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Update_Server
{
    [Route("api/update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        [HttpGet]
        public JsonResult GetVersion()
        {
            var path = "d:/update/Client.zip";
            using var fs = System.IO.File.OpenRead(path);
            using var c = MD5.Create();
            var md5hash = c.ComputeHash(fs);
            var hexstring = md5hash.Aggregate(string.Empty, (res, b) => res + b.ToString("X2"));
            var result = new Data.UpdateInfo
            {
                CurrentVersion="1.0.0.0",
                DownloadURL= "http://localhost:5000/api/file/download",
                ChangelogURL= "http://127.0.0.1/",
                Mandatory = new Data.Mandatory
                {
                    MinVersion="0.0.0.0",
                    Mode=Data.ModeEnum.Forced,
                    Value=true,
                },
                Checksum=new Data.Checksum
                {
                    HashingAlgorithm="MD5",
                    Value= hexstring,
                }
            };
            return new JsonResult(result);
        }
    }
}
