using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace RegistroActividadesE9.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Agregar()
        {
            return View();
        }
        public IActionResult Editar()
        {
            return View();
        }
        public IActionResult Eliminar()
        {
            return View();
        }
        public IActionResult VerMisPublicaciones()
        {
            return View();
        }
        public IActionResult VerHistorialPubli()
        {
            return View();
        }
    }
}
