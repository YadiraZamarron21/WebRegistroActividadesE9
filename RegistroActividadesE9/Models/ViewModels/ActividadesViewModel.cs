namespace RegistroActividadesE9.Models.ViewModels
{
    public class ActividadesViewModel
    {
        public int? id { get; set; }
        public string titulo { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string Departamento { get; set; } = null!;
        public DateOnly? FechaRealizacion { get; set; } //cuando se realizó la actividad
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
