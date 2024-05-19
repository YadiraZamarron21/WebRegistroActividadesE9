using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RegistroActividadesE9.Models.ViewModels;
using Newtonsoft.Json;
using System.Text;
using RegistroActividadesE9.Models.DTOs;

namespace RegistroActividadesE9.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            List<DepartamentosViewModel> departamentoList = new();

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "api/departamento").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                departamentoList = JsonConvert.DeserializeObject<List<DepartamentosViewModel>>(data);
            }
            return View(departamentoList);
        }

        [HttpGet]
        public IActionResult AgregarDepartamento()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AgregarDepartamento(AgregarDepartamentoViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            DepartamentoDTO dto = new()
            {
                nombre = vm.nombre,
                usuario = vm.usuario,
                contrasena = vm.contraseña,
                idSuperior = vm.idSuperior
            };
            string data = JsonConvert.SerializeObject(dto);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

              HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "api/departamento/Post", content).Result;


            return View(vm);
            //    try
            //    {
            //        string data = JsonConvert.SerializeObject(vm);
            //        StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

            //        HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "api/departamento/Post", content).Result;

            //        if (response.IsSuccessStatusCode)
            //        {

            //            return RedirectToAction("Index");
            //        }

            //    }
            //    catch (Exception )
            //    {

            //        return View();
            //    }

            //    return View();
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
