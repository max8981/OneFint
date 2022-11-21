using AntDesign;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;

namespace Update_Server.Controllers
{
    [ApiController]
    [Route("/api/file")]
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IWebHostEnvironment env;
        public FileController(IWebHostEnvironment env)
        {
            this.env = env;
        }
        [HttpPost, Route("upload")]
        [RequestSizeLimit(long.MaxValue)]
        public ResultModel UploadFile([FromForm] IFormCollection formCollection)
        {
            ResultModel result = new()
            {
                Message = "success",
                Code = 0,
                Url = "/api/file/download"
            };
            try
            {
                string uploadPath = System.IO.Path.Combine(env.ContentRootPath, "upload");
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                FormFileCollection fileCollection = (FormFileCollection)formCollection.Files;
                foreach (IFormFile file in fileCollection)
                {
                    string filePath = System.IO.Path.Combine(uploadPath, file.FileName);
                    var p = System.IO.Path.GetDirectoryName(filePath);
                    if (p != null)
                    {
                        if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                        result.Path = p;
                        using FileStream fs = new(filePath, FileMode.OpenOrCreate);
                        file.CopyTo(fs);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Code = 100;
                result.Message = $"文件上传失败：{ex.Message}!";
            }
            return result;
        }
        [HttpPost, Route("uploadfile")]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> UpLoadfileAsync([FromQuery]string path,[FromForm] IFormFile file)
        {
            var tempPath = Path.Combine(env.ContentRootPath, "upload", path);
            if (!System.IO.Directory.Exists(tempPath)) System.IO.Directory.CreateDirectory(tempPath);
            var filePath= Path.Combine(tempPath, file.FileName);
            using var fileStream = System.IO.File.OpenWrite(filePath);
            using var uploadStream = file.OpenReadStream();
            await uploadStream.CopyToAsync(fileStream);
            return new JsonResult(new ResultModel());
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        [HttpGet, Route("download")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        public FileResult Download()
        {

            try
            {
                string filePath = "d:/update/Client.zip";
                Stream stream = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read);
                FileStreamResult actionresult = new FileStreamResult(stream, new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream"));
                actionresult.FileDownloadName = "Client.zip";
                return actionresult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public class ResultModel
        {
            public int Code { get; set; }
            public string Message { get; set; }
            public string Url { get; set; }
            public string Path { get; set; }
        }
    }
}
