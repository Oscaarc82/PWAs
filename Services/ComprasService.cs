

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using PWAs.Models.Conexion;
using PWAs.Models.Reportes;
using PWAs.Sistema;
using System.Data;
using System.Data.SqlClient;

namespace PWAs.Services
{
    public class ComprasService
    {
        private readonly IConfiguration Configuration;

        public ComprasService(IConfiguration config)
        {
            Configuration = config;
        }

        public int CrearCompra(List<CompraDetalles> compraDetalles, Compra compra)
        {
            string sCadenaConexion = Configuration["ConnectionStrings:MainConnection"];
            bool bAlta;
            int idCompra = 0;
            string query, queryD;
            DateTime fecha = DateTime.Now;

            try
            {
                query = "Insert Into Seguridad.dbo.tCompra (iIdUsuario, dFechaCompra, DeTotal) Values" +
                        " (@iId, @dFecha, @deTotal); Select SCOPE_IDENTITY();";

                using (SqlConnection connection = new SqlConnection(sCadenaConexion))
                {
                    using (SqlCommand  command = new SqlCommand(query, connection))
                    {
                        command.Parameters.Add("@iId", SqlDbType.Int).Value = compra.IIdUsuario;
                        command.Parameters.Add("@dFecha", SqlDbType.DateTime).Value = fecha;
                        command.Parameters.Add("@deTotal", SqlDbType.Float).Value = compra.DeTotal;

                        connection.Open();
                        try
                        {
                            idCompra = Convert.ToInt32(command.ExecuteScalar());
                            bAlta = true;
                        }
                        catch { bAlta = false; }
                    }
                }

                if (bAlta)
                {
                    foreach (CompraDetalles item in compraDetalles)
                    {
                        queryD = "Insert Into Seguridad.dbo.tCompraDetalles (iCompra, iProducto, iCantidad, iPrecio) Values" +
                                " (@iCompra, @iProducto, @iCantidad, @dePrecio)";

                        using (SqlConnection connection = new SqlConnection(sCadenaConexion))
                        {
                            using (SqlCommand command = new SqlCommand(queryD, connection))
                            {
                                command.Parameters.Add("@iCompra", SqlDbType.Int).Value = idCompra;
                                command.Parameters.Add("@iProducto", SqlDbType.Int).Value = item.IProducto;
                                command.Parameters.Add("@iCantidad", SqlDbType.Int).Value = item.ICantidad;
                                command.Parameters.Add("@dePrecio", SqlDbType.Float).Value = item.DePrecio;

                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch
            {
                idCompra = -1;
            }
            return idCompra;
        }
    }
}
