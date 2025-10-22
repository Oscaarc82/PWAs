namespace PWAs.Models
{
    public class ProveedorDTO
    {
        public int Iid { get; set; }
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }

    public class ProveedorCreateDTO
    {
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }

    public class ProveedorUpdateDTO
    {
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }
}