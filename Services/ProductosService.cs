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

        public int AltaProducto(ProductoNvo producto)
        {
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            Conexion sCon = new Conexion(sCadenaConexion);
            storedProcedure sp = new storedProcedure(sCadenaConexion, Configuration);
            bool bAlta = true;
            string query = "";
            int iProducto;

            try
            {
                query = "Insert Into Seguridad.dbo.tProducto (sCodigo, sNombre, sDescripcion, iCategoria, dePrecio, iStock, iStockMin, iProveedor) Values " +
                        " (@sCodigo, @sNombre, @sDescripcion, @iCategoria, @dePrecio, @iStock, @iStockMin, @iProveedor); Select  Scope_Identity();";

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
                        command.Parameters.Add("@iProveedor", SqlDbType.Int).Value = producto.iProveedor;

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

        public List<Producto> ListarProductosBajoStock()
        {

            List<Producto> productos = new List<Producto>();
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            string query = "";

            try
            {
                query = "Select iId, sCodigo, sNombre, sDescripcion, iCategoria, dePrecio," +
                    " iStock, iStockMin From Seguridad.dbo.tProducto WHERE iStock <= iStockMin";

                using (SqlConnection connection = new SqlConnection(sCadenaConexion))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var producto = new Producto
                            {
                                IId = reader.GetInt32(0),
                                SCodigo = reader.GetString(1),
                                SNombre = reader.GetString(2),
                                SDescripcion = reader.GetString(3),
                                ICategoria = reader.GetInt32(4),
                                DePrecio = reader.GetDecimal(5),
                                IStock = reader.GetInt32(6),
                                IStockMin = reader.GetInt32(7)
                            };

                            productos.Add(producto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            return productos;
        }
    }
}
