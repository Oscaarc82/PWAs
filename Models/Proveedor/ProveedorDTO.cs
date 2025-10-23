namespace PWAs.Models
{
    public class ProveedorDto
    {
        public int Iid { get; set; }
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }

    public class ProveedorCreateDto
    {
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }

    public class ProveedorUpdateDto
    {
        public string SNombre { get; set; } = string.Empty;
        public string STelefono { get; set; } = string.Empty;
        public string SContacto { get; set; } = string.Empty;
    }
}