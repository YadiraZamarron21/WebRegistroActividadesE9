using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Build.Framework;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Net;
using RegistroActividadesE9.Areas.Admin.Models;
using RegistroActividadesE9.Helpers;
using RegistroActividadesE9.Areas.Admin.Models.Dtos;

namespace RegistroActividadesE9.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class HomeController(HttpClient httpClient, IWebHostEnvironment webHost) : Controller
    {
        private readonly HttpClient httpClient = httpClient;
        private readonly IWebHostEnvironment webHostEnvironment = webHost;


        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] string? departamento, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        {
            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var reponseActividades = httpClient.GetAsync($"/api/Actividad/{userid}");
            var responseDepartamentos = httpClient.GetAsync($"/api/Departamento/{userid}");


            Task.WaitAll(reponseActividades, responseDepartamentos);

            // Verificar si hay error de autenticación
            if (reponseActividades.Result.StatusCode == HttpStatusCode.Unauthorized ||
                responseDepartamentos.Result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("iniciarSesion", "home", new { area = "" });
            }

            var contentActividades = await reponseActividades.Result.Content.ReadAsStringAsync();
            var contentDepartamentos = await responseDepartamentos.Result.Content.ReadAsStringAsync();

            var actividades = JsonConvert.DeserializeObject<IEnumerable<Models.ActividadesViewModel.ActividadModel>>(contentActividades) ?? [];
            var departamentos = JsonConvert.DeserializeObject<IEnumerable<Models.ActividadesViewModel.DepartamentoModel>>(contentDepartamentos) ?? [];

            // Filtrar actividades
            if (fechaInicio != null) actividades = actividades.Where(act => act.fechaRealizacion != null && act.fechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) >= fechaInicio);
            if (fechaFin != null) actividades = actividades.Where(act => act.fechaRealizacion != null && act.fechaRealizacion.Value.ToDateTime(TimeOnly.MinValue) <= fechaFin);

            var vm = new Models.ActividadesViewModel
            {
                Actividades = actividades.Where(act => act.estado != 0),
                Departamentos = departamentos,
                token = token
            };

            return View(vm);            
        }
        [HttpGet]
        public async Task<IActionResult> Agregar()
        {
            var vm = new AgregarActivViewModel();

            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var response = await httpClient.GetAsync($"/api/Departamento/{userid}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var depas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Departamentos>>(content);
                if (depas != null)
                {
                    vm.Departamentos = depas;
                    return View(vm);
                }
            }
            return View(null);
        }


        [HttpPost]
        public async Task<IActionResult> Agregar(AgregarActivViewModel vm)
        {
            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (vm != null)
            {

                if (vm.idDepartamento == 0 || vm.idDepartamento == null)
                {
                    var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                    vm.idDepartamento = int.Parse(userid);
                }

                var converter = new ConverterToBase64(webHostEnvironment);
                 var imagenBase64 = "";
                if (vm.archivo != null)
                { 
                    var ruta = converter.SaveFile(vm.archivo);
                    imagenBase64 = converter.ImageToBase64(ruta);
                }

                var dto = new ActividadDto()
                {
                    titulo = vm.titulo,
                    fechaActualizacion = vm.fechaActualizacion,
                    fechaCreacion = vm.fechaCreacion,
                    fechaRealizacion = vm.fechaRealizacion,
                    descripcion = vm.descripcion,
                    imagen = imagenBase64,
                    idDepartamento = vm.idDepartamento ?? 0,
                    estado = 0,
                    id = 0
                };

                var loginjson = System.Text.Json.JsonSerializer.Serialize(dto);
                var content = new StringContent(loginjson, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/Actividad", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var rresponse = await httpClient.GetAsync($"/api/Departamento/{userid}");
                    if (rresponse.IsSuccessStatusCode)
                    {
                        var content2 = await rresponse.Content.ReadAsStringAsync();

                        var dep = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Departamentos>>(content2);
                        if (dep != null)
                        {
                            vm.Departamentos = dep;
                        }
                    }
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", error);
                    return View(vm);
                }
            }
            return View(vm);
        }


        [HttpGet("Admin/Home/Editar/{id}")]
        public async Task<IActionResult> Editar(int id)
        {
            VerActividadViewModel act = new();
            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var r = await httpClient.GetAsync($"/api/Actividad/{id}");
            if (r.IsSuccessStatusCode)
            {
                var con = await r.Content.ReadAsStringAsync();
                act.activ = JsonConvert.DeserializeObject<Actividad>(con); ;

                return View(act);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(VerActividadViewModel act)
        {
            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Convertir el objeto actividad a JSON
            var converter = new ConverterToBase64(webHostEnvironment);

              var imagenBase64 = "";
            if (act.activ.archivo != null)
            {
                var ruta = converter.SaveFile(act.activ.archivo);
                imagenBase64 = converter.ImageToBase64(ruta);
            }
            if (act.activ.idDepartamento == 0 || act.activ.idDepartamento == null)
            {
                var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

                act.activ.idDepartamento = int.Parse(userid);
            }
            var acti = new ActividadDto()
            {
                id = act.activ.id,
                descripcion = act.activ.descripcion,
                titulo = act.activ.titulo,
                idDepartamento = act.activ.idDepartamento ?? 0,
                fechaCreacion = act.activ.fechaCreacion,
                fechaRealizacion = act.activ.fechaRealizacion,
                imagen = imagenBase64

            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(acti), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync("/api/Actividad", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {

                var r = await httpClient.GetAsync($"/api/Actividad/{act.activ.id}");
                if (r.IsSuccessStatusCode)
                {
                    var con = await r.Content.ReadAsStringAsync();
                    act.activ = JsonConvert.DeserializeObject<Actividad>(con); ;

                    return View(act);
                }
            }
            return View(act);

        }


        [HttpGet("Admin/Home/Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

            var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.DeleteAsync($"/api/Actividad/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al eliminar la actividad");
                return View();
            }
        }

        //Falta lo de borradores

        // [HttpGet]
        //public async Task<IActionResult> Borradores([FromQuery] string? departamento, [FromQuery] DateTime? fechaInicio, [FromQuery] DateTime? fechaFin)
        //{
        //    httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

        //    var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        //    var reponseActividades = httpClient.GetAsync($"/api/Actividad/{userid}");
        //    var responseDepartamentos = httpClient.GetAsync($"/api/Departamento/{userid}");

        //    Task.WaitAll(reponseActividades, responseDepartamentos);

        //    // Verificar si hay error de autenticación
        //    if (reponseActividades.Result.StatusCode == HttpStatusCode.Unauthorized ||
        //        responseDepartamentos.Result.StatusCode == HttpStatusCode.Unauthorized)
        //    {
        //        return RedirectToAction("iniciar-sesion", "home", new { area = string.Empty });
        //    }

        //    var contentActividades = await reponseActividades.Result.Content.ReadAsStringAsync();
        //    var contentDepartamentos = await responseDepartamentos.Result.Content.ReadAsStringAsync();

        //    var actividades = JsonConvert.DeserializeObject<IEnumerable<Models.ActividadesViewModel.actividadModel>>(contentActividades) ?? [];
        //    var departamentos = JsonConvert.DeserializeObject<IEnumerable<Models.ActividadesViewModel.departamentoModel>>(contentDepartamentos) ?? [];

        //    var viewModel = new Models.IndexViewModel
        //    {
        //        actividades = actividades.Where(act => act.estado == 0),
        //        departamentos = departamentos,
        //        token = token
        //    };

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Borrador(AgregarActivViewModel vmW)
        //{
        //    httpClient.BaseAddress = new Uri("https://actividadese9.websitos256.com/");

        //    var token = User.Claims.First(x => x.Type == ClaimTypes.UserData).Value;
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    if (vmW != null)
        //    {
        //        if (vmW.idDepartamento == 0 || vmW.idDepartamento == null)
        //        {
        //            var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

        //            vmW.IdDepartamento = int.Parse(userid);
        //        }

        //        var converter = new ConverterToBase64(webHostEnvironment);

        //        // Suponiendo que tienes una propiedad 'Imagen' en tu ViewModel que contiene la imagen como un byte array
        //        // Aquí debes reemplazar 'vm.Imagen' con la propiedad real que contiene la imagen en tu ViewModel
        //        var imagenBase64 = "";
        //        if (vmW.Archivo != null)
        //        {
        //            var ruta = converter.SaveFile(vmW.Archivo);
        //            imagenBase64 = converter.ImageToBase64(ruta);
        //        }

        //        var actdto = new AddActDto()
        //        {
        //            Titulo = vmW.Titulo,
        //            FechaActualizacion = vmW.FechaActualizacion,
        //            Descripcion = vmW.Descripcion,
        //            FechaCreacion = vmW.FechaCreacion,
        //            FechaRealizacion = vmW.FechaRealizacion,
        //            IdDepartamento = vmW.IdDepartamento ?? 0,
        //            Estado = 0,
        //            Imagen = imagenBase64,
        //            Id = 0
        //        };

        //        var loginjson = System.Text.Json.JsonSerializer.Serialize(actdto);
        //        var content = new StringContent(loginjson, Encoding.UTF8, "application/json");
        //        var response = await httpClient.PostAsync("/api/borrador", content);

        //        if (response.IsSuccessStatusCode)
        //        {

        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            var userid = User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        //            var rresponse = await httpClient.GetAsync($"/api/Departamentos/{userid}");
        //            if (rresponse.IsSuccessStatusCode)
        //            {
        //                var content2 = await rresponse.Content.ReadAsStringAsync();

        //                var depas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Departamentos>>(content2);
        //                if (depas != null)
        //                {
        //                    vmW.Departamentos = depas;
        //                }

        //            }
        //            var error = await response.Content.ReadAsStringAsync();
        //            ModelState.AddModelError("", error);
        //            return View(vmW);
        //        }
        //    }
        //    return View(vmW);
        //}
    }
}
