using Microsoft.AspNetCore.Mvc;

namespace IMS_System.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
