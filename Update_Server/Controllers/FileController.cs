using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        public ResultModel UploadFile([FromForm] IFormCollection formCollection)
        {
            ResultModel result = new ResultModel();
            result.Message = "success";
            result.Code = 0;
            result.Url = "/api/file/download";
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
                    if (!Directory.Exists(p)) Directory.CreateDirectory(p);
                    result.Path = p;
                    using FileStream fs = new(filePath, FileMode.OpenOrCreate);
                    file.CopyTo(fs);
                    break;
                }
            }
            catch (Exception ex)
            {
                result.Code = 100;
                result.Message = $"文件上传失败：{ex.Message}!";
            }
            return result;
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
