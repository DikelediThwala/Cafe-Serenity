using Microsoft.AspNetCore.Mvc;

namespace Coffee_Shop.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
