using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWAs.Models.Usuarios
{
    [Table("tRoles")]
    public class Rol
    {
        [Key]
        public int IIdRol {  get; set; }
        public string? SRole { get; set; }
        public string? SDescripcion { get; set; }
    }
}
