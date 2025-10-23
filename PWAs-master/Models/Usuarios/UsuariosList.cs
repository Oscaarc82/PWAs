using System.ComponentModel.DataAnnotations;

namespace PWAs.Models.Usuarios
{
    public class UsuariosList
    {
        public int IID { get; set; }
        public string? SNombre { get; set; }

        public string? SEmail { get; set; }
        public string? sRol { get; set; }
    }
}
