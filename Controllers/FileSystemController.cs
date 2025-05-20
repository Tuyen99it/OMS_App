using Microsoft.AspNetCore.Mvc;
using elFinder.NetCore.Drivers.FileSystem;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http.Extensions;

using elFinder.NetCore;

namespace OMS_App.Controllers
{
    //[Authorize] // Have to login can access
    [Route("el-finder-file-system")]
    public class FileSystemController : Controller
    {
        private readonly ILogger<FileSystemController> _logger;
        private IWebHostEnvironment _env;
        public FileSystemController(ILogger<FileSystemController> logger, IWebHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }
        // Create method to client site connect to backend
        [Route("connector")]
        public async Task<IActionResult> Connector()
        {
            var connector = GetConnector();
            return await connector.ProcessAsync(Request);
        }
        // Địa chỉ để truy vấn thumbnail
        // /el-finder-file-system/thumb
        [Route("thumb/{hash}")]
        public async Task<IActionResult> Thumbs(string hash)
        {
            var connector = GetConnector();
            return await connector.GetThumbnailAsync(HttpContext.Request, HttpContext.Response, hash);
        }

        private Connector GetConnector()
        {
            // RootFile is wwwroot/files → make sure in wwwroot have "files" folder
            string pathroot = "files";
            var driver = new FileSystemDriver();
            string absoluteUrl = UriHelper.BuildAbsolute(Request.Scheme, Request.Host);
            var uri = new Uri(absoluteUrl);


            //...wwwroot/uploads
            string rootDirectory = Path.Combine(_env.WebRootPath, pathroot);


            // https://localhost:5001/files
            string url = $"{uri.Scheme}://{uri.Authority}/{pathroot}/";
            string urlthumb = $"{uri.Scheme}://{uri.Authority}/el-finder-file-system/thum/";

            var root = new RootVolume(rootDirectory, url, urlthumb)
            {
                //IsReadOnly = !User.IsInRole("Administrators")
                IsReadOnly = false, // Can be readonly according to user's membership permission
                IsLocked = false, // If locked, files and directories cannot be deleted, renamed or moved
                Alias = "Files", // Beautiful name given to the root/home folder
                //MaxUploadSizeInKb = 2048, // Limit imposed to user uploaded file <= 2048 KB
                //LockedFolders = new List<string>(new string[] { "Folder1" }
                ThumbnailSize = 100,
            };
            driver.AddRoot(root);
            return new Connector(driver)
            {
                // This allows support for the "onlyMimes" option on the client.
                MimeDetect = MimeDetectOption.Internal
            };
        }
    }
}