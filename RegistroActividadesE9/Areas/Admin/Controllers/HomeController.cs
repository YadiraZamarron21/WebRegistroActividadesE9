using Microsoft.AspNetCore.Mvc;

namespace RegistroActividadesE9.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AgregarDepartamento()
        {
            return View();
        }
        public IActionResult EditarDepartamento()
        {
            return View();
        }
        public IActionResult EliminarDepartamento()
        {
            return View();
        }
    }
}
