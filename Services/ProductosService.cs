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

        public int AltaProducto(Producto producto)
        {
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenaConexion);
            storedProcedure sp = new storedProcedure(sCadenaConexion, Configuration);
            bool bAlta = true;
            string query = "";
            int iProducto;

            try
            {
                query = "Insert Into Seguridad.dbo.tProducto (sCodigo, sNombre, sDescripcion, iCategoria, dePrecio, iStock, iStockMin) Values " +
                        " (@sCodigo, @sNombre, @sDescripcion, @iCategoria, @dePrecio, @iStock, @iStockMin); Select  Scope_Identity();";

                using (SqlConnection connection = new SqlConnection(sCadenaConexion))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@sCodigo", SqlDbType.VarChar).Value = producto.SCodigo;
                        command.Parameters.Add("@sNombre", SqlDbType.VarChar).Value = producto.SNombre;
                        command.Parameters.Add("@sDescripcion", SqlDbType.VarChar).Value = producto.SDescripcion;
                        command.Parameters.Add("@iCategoria", SqlDbType.Int).Value = producto.ICategoria;
                        command.Parameters.Add("@dePrecio", SqlDbType.Decimal).Value = producto.DePrecio;
                        command.Parameters.Add("@iStock", SqlDbType.Int).Value = producto.IStock;
                        command.Parameters.Add("@iStockMin", SqlDbType.Int).Value = producto.IStockMin;

                        connection.Open();

                        try
                        {
                            iProducto = Convert.ToInt32(command.ExecuteScalar());
                        }
                        catch 
                        {
                            iProducto = -1;
                        }
                    }
                }
            }
            catch
            {
                iProducto = -1;
            }
            return iProducto;
        }
    }
}
