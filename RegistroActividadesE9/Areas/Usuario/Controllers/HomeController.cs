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


        [HttpGet]
        public IActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Agregar( AgregarActividadViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.titulo))
            {
                ModelState.AddModelError("", "Escriba el titulo de la actividad");
            }
            if (string.IsNullOrWhiteSpace(vm.descripcion))
            {
                ModelState.AddModelError("", "Escriba una descripcion de la actividad");
            }
      
            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(vm);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PostAsync(_client.BaseAddress + "api/actividad", content).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }


            return View(vm);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            try
            {
                AgregarActividadViewModel act = new AgregarActividadViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/api/actividad/Get/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    act = JsonConvert.DeserializeObject<AgregarActividadViewModel>(data);
                }
                return View(act);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Editar(AgregarActividadViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.titulo))
            {
                ModelState.AddModelError("", "Escriba el titulo de la actividad");
            }
            if (string.IsNullOrWhiteSpace(vm.descripcion))
            {
                ModelState.AddModelError("", "Escriba una descripcion de la actividad");
            }

            if (ModelState.IsValid)
            {
                string data = JsonConvert.SerializeObject(vm);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = _client.PutAsync(_client.BaseAddress + "/api/actividad/Put", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            try
            {
                ActividadesViewModel acti = new ActividadesViewModel();
                HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "api/actividad/Get/" + id).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    acti = JsonConvert.DeserializeObject<ActividadesViewModel>(data);

                }
                return View(acti);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult EliminarC(int id)
        {
            try
            {
                HttpResponseMessage response = _client.DeleteAsync(_client.BaseAddress + "api/actividad/Delete/" + id).Result;

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
