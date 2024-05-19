namespace RegistroActividadesE9.Models.DTOs
{
    public class DepartamentoDTO
    {
        public int id { get; set; }
        public string nombre { get; set; } = null!;
        public string usuario { get; set; } = null!;
        public string contrasena { get; set; } = null!;
        public int idSuperior { get; set; }
    }
}
