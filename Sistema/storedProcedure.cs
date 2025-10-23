using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace PWAs.Sistema
{
    public class StoredProcedure
    {
        private SqlConnection conexion;
        SqlCommand comando = null;
        private string conn;
        private readonly IConfiguration Configuration;

        public string Conn
        {
            get { return conn; }
            set { conn = value; }
        }
        public StoredProcedure(string conn, IConfiguration config)
        {
            this.conn = conn;
            Configuration = config;
        }

        public void establecerConexion()
        {
            conexion = new SqlConnection(conn);

        }

        public string bulkTablas(string type, DataTable dt, string procedimiento)
        {
            establecerConexion();
            try
            {
                comando = new SqlCommand(procedimiento, conexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue(type, dt);
                comando.CommandTimeout = 7200000;
                conexion.Open();
                comando.ExecuteReader();

                return "0";
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return "1";
            }
            finally
            {
                if (comando != null)
                    comando.Dispose();
                conexion.Close();
            }

        }

        public bool ejecutaSQL(string query)
        {
            establecerConexion();
            comando = new SqlCommand(query, conexion);
            comando.CommandType = CommandType.Text;

            try
            {
                conexion.Open();
                comando.ExecuteReader();
                conexion.Close();

                return true;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return false;
            }

        }
        public List<string> recuperaRegistros(string query)
        {
            SqlDataReader resultados = null;
            List<string> resultado = new();
            establecerConexion();
            comando = new SqlCommand(query, conexion);
            comando.CommandType = CommandType.Text;
            try
            {
                conexion.Open();
                resultados = comando.ExecuteReader();
                if (resultados.HasRows)
                {
                    while (resultados.Read())
                    {
                        for (int i = 0; i < resultados.FieldCount; i++)
                        {
                            resultado.Add(resultados.GetValue(i).ToString());
                        }
                    }
                }
                conexion.Close();

                return resultado;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return new List<string>();
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }

        }

        public List<Dictionary<string, string>> recuperaRegistroslst(string query)
        {
            SqlDataReader resultados = null;
            var resultado = new List<Dictionary<string, string>>();
            establecerConexion();
            comando = new SqlCommand(query, conexion);
            comando.CommandType = CommandType.Text;
            try
            {
                conexion.Open();
                resultados = comando.ExecuteReader();
                if (resultados.HasRows)
                {
                    while (resultados.Read())
                    {
                        var fila = new Dictionary<string, string>();
                        for (int i = 0; i < resultados.FieldCount; i++)
                        {
                            fila[resultados.GetName(i)] = resultados.GetValue(i).ToString();
                        }
                        resultado.Add(fila);
                    }
                }
                conexion.Close();

                return resultado;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return new List<Dictionary<string, string>>();
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }
        }

        public string recuperaValor(string query)
        {
            SqlDataReader resultados = null;
            string resultado = "";
            establecerConexion();

            comando = new SqlCommand(query, conexion);
            comando.CommandType = CommandType.Text;

            try
            {
                conexion.Open();
                resultados = comando.ExecuteReader();
                if (resultados.HasRows)
                {
                    while (resultados.Read())
                    {
                        resultado = resultados.GetValue(0).ToString();
                    }
                }
                conexion.Close();

                return resultado;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return "-1";
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }

        }

        public string construirMenu(int idUsuario, int idSeccion, string idSistema, string idUs, int opcion, string encSeccion)
        {
            SqlDataReader resultados = null;
            establecerConexion();
            if (opcion == 1)
            {
                comando = new SqlCommand("sp_menuH", conexion);
            }
            else if (opcion == 2)
            {
                comando = new SqlCommand("sp_menuC", conexion);
            }
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.Add("@idUsuario", SqlDbType.Int).Value = idUsuario;
            comando.Parameters.Add("@idSeccion", SqlDbType.Int).Value = idSeccion;
            comando.Parameters.Add("@idSistema", SqlDbType.VarChar).Value = idSistema;
            comando.Parameters.Add("@idUs", SqlDbType.VarChar).Value = idUs;
            comando.Parameters.Add("@encSeccion", SqlDbType.VarChar).Value = encSeccion;

            string result = "";

            try
            {
                conexion.Open();
                resultados = comando.ExecuteReader();
                while (resultados.Read())
                {
                    result = resultados.GetValue(0).ToString();
                }
                conexion.Close();

                return result;
            }
            catch (SqlException e)
            {
                return e.Message.ToString();
            }
        }

        #region GENERICO
        #region EJECUAR STORE PROCEDURE
        /// <summary>
        /// Ejecuta un store de manera generica.
        /// </summary>
        /// <param name="lstParametros">Lista con el nombre los parametros del store procedure.</param>
        /// <param name="lstDatos">Lista con los datos en orden acorde a la lista de paramentros.</param>
        /// <param name="sStore">Nombre del store procedure.</param>
        /// <returns>Indicador segun la operacion en el store, y -2 cuando ocurre error.</returns>
        public string EjecutaStoreProc(List<string> lstParametros, List<string> lstDatos, string sStore)
        {
            SqlDataReader resultados = null;
            string sResultado = "";
            establecerConexion();
            try
            {
                comando = new SqlCommand(sStore, conexion);
                comando.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < lstParametros.Count; i++)
                {
                    comando.Parameters.AddWithValue(lstParametros[i].ToString(), lstDatos[i].ToString());
                }
                comando.CommandTimeout = 7200000;
                conexion.Open();
                resultados = comando.ExecuteReader();
                if (resultados.HasRows)
                {
                    while (resultados.Read())
                    {
                        sResultado = resultados.GetValue(0).ToString();
                    }
                }
                conexion.Close();

                return sResultado;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return "-2";
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }

        }
        #endregion

        #region EJECUTAR STORE PROCEDURE DATASET
        /// <summary>
        /// Ejecutar un store que reciba de parametros ya sea algun datatable o datos, de otro tipo que se enviaran como string.
        /// </summary>
        /// <param name="lstParametros">Lista de los nombres de los parametros de SP, iniciando con el nombre del paramentro del DataTable.</param>
        /// <param name="lstDatos">Lista de los datos de los parametros extra.</param>
        /// <param name="sStore">Nombre del Store Procedure.</param>
        /// <param name="dtsDatos">DataSet con los Datables a agregar al SP, mismo que tienen con los datos.</param>
        /// <returns>1:Exito, -1:Error.</returns>
        public string EjecutaStoreDataSet(List<string> lstParametros, List<string> lstDatos, string sStore, DataSet dtsDatos)
        {
            SqlDataReader resultados = null;
            string sResultado = "";
            establecerConexion();
            try
            {
                comando = new SqlCommand(sStore, conexion);
                comando.CommandType = CommandType.StoredProcedure;
                int iContador = 0;
                for (int i = 0; i < dtsDatos.Tables.Count; i++)
                {
                    comando.Parameters.AddWithValue(lstParametros[i].ToString(), dtsDatos.Tables[i]);
                    iContador = i;
                }
                iContador++;
                for (int i = 0; i < lstParametros.Count - iContador; i++)
                {
                    comando.Parameters.AddWithValue(lstParametros[i + iContador].ToString(), lstDatos[i].ToString());
                }

                comando.CommandTimeout = 7200000;
                conexion.Open();
                resultados = comando.ExecuteReader();
                if (resultados.HasRows)
                {
                    while (resultados.Read())
                    {
                        sResultado = resultados.GetValue(0).ToString();
                    }
                }
                conexion.Close();

                return sResultado;
            }
            catch (SqlException ex)
            {
                Console.Write(ex);
                return "-1";
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }

        }
        #endregion

        #endregion
    }
}
