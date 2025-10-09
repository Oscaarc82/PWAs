using PWAs.Models.Conexion;
using PWAs.Models.Reportes;
using PWAs.Sistema;
<<<<<<< HEAD
using System.Data;
using System.Data.SqlClient;
=======
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
>>>>>>> edbb295 (Subiendo mi proyecto a Miguel)

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
<<<<<<< HEAD
=======

        //Listar productos con Stock minimos
        public List<Producto> ListarProductosBajoStock()
        {
            
            List<Producto> productos = new List<Producto>();
            sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            string query = "";

            try
            {
                query = "SELECT iId, sCodigo, sNombre, sDescripcion, iCategoria, dePrecio," +
                    " iStock, iStockMin FROM tProducto WHERE iStock <= iStockMin";

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
            catch (Exception ex) {
            
                Console.WriteLine(ex.Message);
            }

            return productos;
        }
        

>>>>>>> edbb295 (Subiendo mi proyecto a Miguel)
    }
}
