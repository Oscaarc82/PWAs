using Newtonsoft.Json;

namespace PWAs.Models.Usuarios
{
    public class UsuarioNvo
    {
        public string? SNombre { get; set; }
        public string? SEmail { get; set; }
        public string? SPasswd { get; set; }
        [JsonRequired] public DateTime DFechaC { get; set; }
        [JsonRequired] public DateTime DFechaA { get; set; }
        [JsonRequired] public int IEstatus { get; set; }
        [JsonRequired] public int iRol { get; set; }
    }
}
