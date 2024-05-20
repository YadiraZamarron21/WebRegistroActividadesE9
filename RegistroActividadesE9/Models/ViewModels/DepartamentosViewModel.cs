namespace RegistroActividadesE9.Models.ViewModels
{
    public class DepartamentosViewModel
    {
        public int id { get; set; }
        public string nombre { get; set; } = null!;
        public string usuario { get; set; } 
        public string contraseña { get; set; } 
        public int idSuperior { get; set; }

    }
}
