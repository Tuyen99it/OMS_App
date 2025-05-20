using Microsoft.AspNetCore.Mvc;

namespace OMS_App.Controllers
{
    //[Authorize] // Have to login can access
    [Route("/file-manager")]
    public class FileManagerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }

}