using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Net;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;

namespace Update_Server.Controllers
{
    [Route("api/update")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        [HttpGet,Route("GetVersion")]
        //[Produces("application/xml")]
        public IActionResult GetVersion(string? key)
        {
            if (key != null)
                return new JsonResult(Data.Project.GetProject(key).UpdateInfo);
            else
                return NotFound();
        }
        [HttpGet, Route("Changelog")]
        public IActionResult GetChangelog(string? key)
        {
            return RedirectToPage("/Changelog", new { key=key});
        }
        [HttpGet, Route("GetPackage")]
        [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
        public FileResult Download(string? key)
        {
            var project = Data.Project.GetProject(key);
            if (project.Packages.Count > 0)
            {
                var package = project.Packages.OrderByDescending(_ => _.Version).First();
                try
                {
                    string filePath = package.PackagePath;
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
            else
            {
                return null;
            }
        }
    }
}
