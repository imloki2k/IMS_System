using Microsoft.AspNetCore.Mvc;

namespace IMS_System.Controllers
{
    public class LandingPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
