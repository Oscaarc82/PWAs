using Microsoft.Data.SqlClient;
using NetTopologySuite.Geometries;
using PWAs.Models.Conexion;
using PWAs.Models.Usuarios;
using PWAs.Sistema;
using System.Data;


namespace PWAs.Services
{
    public class UsuariosService
    {
        private IConfiguration Configuration;
        string sCadenadeConexion;

        public UsuariosService(IConfiguration config)
        {
            Configuration = config;
        }

        public bool AltaUsuario(UsuarioNvo usuario, string sSalt)
        {
            sCadenadeConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenadeConexion);
            storedProcedure sp = new storedProcedure(sCadenadeConexion, Configuration);
            bool bAlta = true;
            string query = "";

            DateTime fecha = DateTime.Now;
            string hsPasswd = BCrypt.Net.BCrypt.HashPassword(usuario.SPasswd);

            try
            {
                /*query = "Insert Into Seguridad.dbo.tUsuarios (sNombre, sEmail, sPasswd, dFechaC, iEstatus, iIdRol) " +
                        " Values ('" +  usuario.SNombre + "', '" + usuario.SEmail + "', '" + hsPasswd + "', '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "', 1, " + usuario.iRol + ")";*/
                query = "Insert Into Seguridad.dbo.tUsuarios (sNombre, sEmail, sPasswd, dFechaC, iEstatus, iIdRol) Values" +
                        " (@sNombre, @sEmail, @sPasswd, @dFechaC, @iEstatus, @iIdRol)";

                using (SqlConnection connection = new SqlConnection(sCadenadeConexion))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@sNombre", SqlDbType.VarChar).Value = usuario.SNombre;
                        command.Parameters.Add("@sEmail", SqlDbType.VarChar).Value = usuario.SEmail;
                        command.Parameters.Add("@sPasswd", SqlDbType.VarChar).Value = hsPasswd;
                        command.Parameters.Add("@dFechaC", SqlDbType.DateTime).Value = fecha;
                        command.Parameters.Add("@iEstatus", SqlDbType.Int).Value = 1;
                        command.Parameters.Add("@iIdRol", SqlDbType.Int).Value = usuario.iRol;

                        connection.Open();
                        bAlta = command.ExecuteNonQuery() > 0;
                    }
                }
            } catch
            {
                bAlta = false;

                string queryErr = "";
                sp.ejecutaSQL(queryErr);
            }
            return bAlta;
        }

        public List<UsuariosList> ListarUsuarios()
        {
            sCadenadeConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenadeConexion);
            storedProcedure sp = new storedProcedure(sCadenadeConexion, Configuration);
            List<string> res = new List<string>();
            List<UsuariosList> lstUsuarios = new List<UsuariosList>();
            string query = "";

            try
            {
                query = "Select tu.iID, tu.sNombre, tu.sEmail, tr.sRole From Seguridad.dbo.tUsuarios as tu" +
                        " Join Seguridad.dbo.tRoles as tr" +
                        " On tr.iIdRol = tu.iIdRol";

                res = sp.recuperaRegistros(query);

                if (res.Count > 0)
                {
                    for (int i = 0; i < res.Count; i+=4)
                    {
                        UsuariosList user = new UsuariosList
                        {
                            IID = int.Parse(res[i]),
                            SNombre = res[i + 1],
                            SEmail = res[i + 2],
                            sRol = res[i + 3]
                        };

                        lstUsuarios.Add(user);
                    }
                }
            } catch
            {
                lstUsuarios.Add(new UsuariosList());
            }
            return lstUsuarios;
        }

        public bool EliminarUsuario(int iIdUsuario, int iAcccion)
        {
            sCadenadeConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenadeConexion);
            storedProcedure sp = new storedProcedure(sCadenadeConexion, Configuration);
            bool bBaja = true;
            string query = "";

            try
            {
                query = "Delete From Seguridad.dbo.tUsuarios " +
                        " Where iID = " + iIdUsuario + "";

                bBaja = sp.ejecutaSQL(query);
            }
            catch
            {
                bBaja = false;
            }
            return bBaja;
        }

        public bool ActualizarUsuario(int iIdUsuario, string sNombre, string sEmail)
        {
            sCadenadeConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenadeConexion);
            storedProcedure sp = new storedProcedure(sCadenadeConexion, Configuration);
            bool bActualizacion = true;
            string query;

            try
            {
                query = "Update tUsuarios" +
                        " Set " +
                        " sNombre = '" + sNombre + "'," +
                        " sEmail = '" + sEmail + "'" +
                        " Where iID = " + iIdUsuario + "";

                bActualizacion = sp.ejecutaSQL(query);
            }
            catch
            {
                bActualizacion = false;
            }
            return bActualizacion;
        }
    }
}
