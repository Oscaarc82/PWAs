using PWAs.Models.Conexion;
using PWAs.Sistema;
using System.Data;
using System.Data.SqlClient;

namespace PWAs.Services
{
    public class MovimientosService
    {
        private IConfiguration Configuration;
        string sCadenaConexion;

        public MovimientosService(IConfiguration config)
        {
            Configuration = config;
        }

        public bool registrarMovimiento(int tipo, int iProducto, int iCantidad, string referencia, string responsable)
        {
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new(sCadenaConexion);
            storedProcedure sp = new storedProcedure(sCadenaConexion, Configuration);
            bool bBandera;
            string query;

            try
            {
                query = "Insert Into Seguridad.dbo.tMovimiento (dFechaM, iTipo, iProducto, iCantidad, sReferencia, sResponsable) Values" +
                        " (@dFechaM, @iTipo, @iProducto, @iCantidad, @sReferencia, @sResponsable)";

                using (SqlConnection connection = new(sCadenaConexion))
                {
                    using (SqlCommand command = new(query, connection))
                    {
                        command.Parameters.Add("@dFechaM", SqlDbType.DateTime).Value = DateTime.UtcNow;
                        command.Parameters.Add("@iTipo", SqlDbType.Int).Value = tipo;
                        command.Parameters.Add("@iProducto", SqlDbType.Int).Value = iProducto;
                        command.Parameters.Add("@iCantidad", SqlDbType.Int).Value = iCantidad;
                        command.Parameters.Add("@sReferencia", SqlDbType.VarChar).Value = referencia;
                        command.Parameters.Add("@sResponsable", SqlDbType.VarChar).Value = responsable;

                        connection.Open();
                        bBandera = command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                bBandera = false;
            }
            return bBandera;
        }
    }
}
