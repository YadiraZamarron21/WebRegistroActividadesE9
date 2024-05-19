var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();

//Autenticacion

//sericios



var app = builder.Build();

app.UseStaticFiles();
app.MapControllerRoute(
      name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );


//agg lo que falta
app.MapDefaultControllerRoute();

app.Run();
