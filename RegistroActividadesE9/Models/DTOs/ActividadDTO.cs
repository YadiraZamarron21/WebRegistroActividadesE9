namespace RegistroActividadesE9.Models.DTOs
{
    public class ActividadDTO
    {
        public int id { get; set; }
        public string titulo { get; set; } = null!;
        public string descripcion { get; set; }= null!;
        public int departamento { get; set; } 
        public DateOnly fechaRealizacion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public int estado { get; set; }
    }
}
