using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RegistroActividadesE9.Models.ViewModels;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace RegistroActividadesE9.Areas.Usuario.Controllers
{
    [Area("Usuario")]
    public class HomeController : Controller
    {
        Uri baseUri = new Uri("https://actividadese9.websitos256.com/");

        private readonly HttpClient _client;
        public HomeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseUri;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ActividadesViewModel> actividadesList = new();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "api/actividad/publicada").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                actividadesList = JsonConvert.DeserializeObject<List<ActividadesViewModel>>(data);
            }
            return View(actividadesList);
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
