using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using RegistroActividadesE9.Models.ViewModels;
using Newtonsoft.Json;
using System.Text;
using RegistroActividadesE9.Models.DTOs;
using Microsoft.Build.Framework;

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

            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "api/Departamento").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                departamentoList = JsonConvert.DeserializeObject<List<DepartamentosViewModel>>(data);
            }
            return View(departamentoList);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar(AgregarDepartamentoViewModel vm)
        {
            try
            {
                string data = JsonConvert.SerializeObject(vm);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "api/departamento", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {
                return View();
            }

            return View();
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                AgregarDepartamentoViewModel departamento = new AgregarDepartamentoViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/api/departamento/Get/"+ id ).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    departamento = JsonConvert.DeserializeObject<AgregarDepartamentoViewModel>(data);
                }
                return View(departamento);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Editar(AgregarDepartamentoViewModel vm)
        {
            try
            {
                string data = JsonConvert.SerializeObject(vm);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/api/departamento/Put", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                return View();
            }
         
            return View();
        }


        public IActionResult EliminarDepartamento()
        {
            return View();
        }
    }
}
