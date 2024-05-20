namespace RegistroActividadesE9.Models.ViewModels
{
    public class ActividadesViewModel
    {
        public IEnumerable<DepartamentoModel> departamento { get; set; } = null!;
    }
    public class DepartamentoModel
    {
        public string departamento { get; set; } = null!;
        public IEnumerable<actividadesModel> actividades { get; set; } = null!;
    }
    public class actividadesModel
    {
        public int? id { get; set; }
        public string titulo { get; set; } = null!;
        public string? descripcion { get; set; }
        public DateOnly? fechaRealizacion { get; set; } //cuando se realizó la actividad
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }
}
