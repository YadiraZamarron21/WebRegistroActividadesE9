using Microsoft.AspNetCore.Mvc;

namespace RegistroActividadesE9.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
