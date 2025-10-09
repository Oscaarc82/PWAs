namespace PWAs.Models.Usuarios
{
    public class UsuarioNvo
    {
        public string? SNombre { get; set; }

        public string? SEmail { get; set; }

        public string? SPasswd { get; set; }

        public DateTime DFechaC { get; set; }

        public DateTime DFechaA { get; set; }

        public int IEstatus { get; set; }

        public int iRol { get; set; }
    }
}
