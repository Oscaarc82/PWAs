using PWAs.Models.Conexion;
using PWAs.Models.Reportes;
using PWAs.Sistema;
using System.Data;
using System.Data.SqlClient;

namespace PWAs.Services
{
    public class ProductosService
    {
        private IConfiguration Configuration;
        string sCadenaConexion;

        public ProductosService(IConfiguration config)
        {
            Configuration = config;
        }

        public bool AltaProducto(Producto producto)
        {
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenaConexion);
            storedProcedure sp = new storedProcedure(sCadenaConexion, Configuration);
            bool bAlta = true;
            string query = "";

            try
            {
                query = "Insert Into Seguridad.dbo.tProducto (sNombre, dePrecio, iStock, isDisponible) Values " +
                        " (@sNombre, @dePrecio, @iStock, @isDisponible)";

                using (SqlConnection connection = new SqlConnection(sCadenaConexion))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@sNombre", SqlDbType.VarChar).Value = producto.SNombre;
                        command.Parameters.Add("@dePrecio", SqlDbType.Decimal).Value = producto.DePrecio;
                        command.Parameters.Add("@iStock", SqlDbType.Int).Value = producto.IStock;
                        command.Parameters.Add("@isDisponible", SqlDbType.Int).Value = producto.IsDisponible;

                        connection.Open();

                        bAlta = command.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                bAlta = false;
            }
            return bAlta;
        }
    }
}
