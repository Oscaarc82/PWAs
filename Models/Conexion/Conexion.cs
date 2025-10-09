using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using PWAs.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace PWAs.Models.Conexion
{
    public class Conexion
    {
        private ConnectionState oConexionEstadoAbierto = new ConnectionState();
        private static readonly IConfiguration Configuration;
        #region Ejecuta Sentencia SQL
        /**
         * Ejecuta una sentencia SQL
         * Regresa true si se ejuctó o false si no se ejecuta la consulta
        **/
        public bool EjecutarSQL(SqlConnectionStringBuilder builder, string sQueryString)
        {
            var ctx = new AppDbContextEmp(builder.ConnectionString);
            DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
            string resultado = "";

            try
            {
                cmd.CommandText = sQueryString;
                cmd.CommandType = CommandType.Text;
                cmd.Connection.Open();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            resultado = reader.GetValue(0).ToString();
                        }
                    }
                }

                cmd.Connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();
            }
        }
        #endregion

        #region RecuperaValoreNum
        /**
         * Ejecuta una sentencia SQL
         * Regresa el int de la consulta
        **/
        public int RecuperaValorNum(SqlConnectionStringBuilder bulder, string sQueryString)
        {

            var ctx = new AppDbContextEmp(bulder.ConnectionString);
            DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
            int resultado;
            try
            {
                cmd.CommandText = sQueryString;
                cmd.CommandType = CommandType.Text;

                cmd.Connection.Open();
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 72000;


                object resultados = cmd.ExecuteScalar();
                resultado = Convert.ToInt32(resultados);

                cmd.Connection.Close();
                return resultado;

            }
            catch (SqlException ex)
            {
                return 0;
            }
            finally
            {
                cmd.Connection.Close();
                cmd.Connection.Dispose();

            }

        }
        #endregion

        #region getValues
        /**
         * Ejecuta una sentencia SQL
         * Regresa un objeto datatable
        **/
        public DataTable GetValues(SqlConnectionStringBuilder bulder, string sQueryString)
        {
            DataTable ds = new DataTable();

            var ctx = new AppDbContextEmp(bulder.ConnectionString);
            try
            {

                using (DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand())
                {

                    cmd.CommandText = sQueryString;
                    cmd.CommandType = CommandType.Text;

                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 72000;


                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            ds.Load(reader);
                        }
                    }
                    cmd.Connection.Close();

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            return ds;
        }
        #endregion

        #region recuperaValor
        /**
         * Ejecuta una sentencia SQL
         * Regresa el string de la consulta
        **/
        public string recuperaValor(SqlConnectionStringBuilder bulder, string query)
        {
            var ctx = new AppDbContextEmp(bulder.ConnectionString);
            string sResultado = "";
            try
            {
                using (DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Connection.Open();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    cmd.CommandTimeout = 30000;
                    using (DbDataReader resultados = cmd.ExecuteReader())
                    {

                        if (resultados.HasRows)
                        {
                            while (resultados.Read())
                            {
                                sResultado = resultados.GetValue(0).ToString();
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return "-1";
            }
            return sResultado;
        }
        #endregion

        #region Agregar Parametro a un Procedimiento Almacenado
        /**
         * Agregar Parametro
        **/
        public static void AgregarParametro(DbCommand cm, string name, object value)
        {
            var parameter = cm.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            cm.Parameters.Add(parameter);
        }
        #endregion

        #region RecuperarRegistros
        /**
         * Ejecuta una sentencia SQL
         * Regresa una lista de string con el resultado de la consulta
        **/
        public List<string> recuperaRegistros(SqlConnectionStringBuilder bulder, string query)
        {

            var ctx = new AppDbContextEmp(bulder.ConnectionString);
            List<string> resultado = new List<string>();


            try
            {
                using (DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Connection.Open();
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 30000;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    resultado.Add(reader.GetValue(i).ToString());
                                }
                            }

                        }
                        //conexion.Close();
                        return resultado;
                    }
                    cmd.Connection.Close();
                }

            }
            catch (SqlException ex)
            {
                return null;
            }
        }
        #endregion

        #region getObject
        /**
         * Ejecuta una sentencia SQL
         * Regresa una lista de objetos con el resultado de la consulta
        **/
        public List<T> getObject<T>(SqlConnectionStringBuilder bulder, string query) where T : new()
        {
            List<T> Lista = new List<T>();

            var ctx = new AppDbContextEmp(bulder.ConnectionString);
            try
            {
                using (DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand())
                {
                    cmd.Connection.Open();
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 30000;

                    using (DbDataReader resultados = cmd.ExecuteReader())
                    {

                        while (resultados.Read())
                        {
                            var Data = new T();
                            PropertyInfo[] Properties = Data.GetType().GetProperties();
                            foreach (var p in Properties)
                            {
                                p.SetValue(Data, resultados.GetValue(resultados.GetOrdinal(p.Name)), null);
                            }
                            Lista.Add(Data);
                        }
                    }
                    cmd.Connection.Close();
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            return Lista;
        }
        #endregion

        #region bulkTablaGenerico
        public string BulkTablaGenerico(string nomProcedure, Object[] oParametros, SqlConnectionStringBuilder builder)
        {
            var ctx = new AppDbContextEmp(builder.ConnectionString);
            DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
            try
            {
                cmd.CommandText = nomProcedure;
                cmd.Connection.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 7200000;
                for (int i = 0; i < oParametros.Length; i++)
                {
                    AgregarParametro(cmd, "@param" + i, oParametros[i]);
                }

                var result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "";
            }
            catch (SqlException ex)
            {
                return "-1";
            }
            finally
            {
                cmd.Connection.Close();
            }

        }
        #endregion

        #region EJECUTAR STORE PROCEDURE DINAMICO
        // Metodos para llamar procedimientos almacenados.

        /// <summary>
        /// Metodo para inicializar un procedimiento almacenado.
        /// </summary>
        /// <param name="sNombreSP">Nombre del store procedure a inicializar.</param>
        /// <param name="iTimeout">Timpo maximo de espera.</param>
        /// <returns>String con la respuesta si se inicializo correctemente o no.</returns>
        public string[] GenerarSP(string sNombreSP, int iTimeout, string sVariableProc, string sValor, string sVariableProc2, DataTable dtlValor, SqlConnectionStringBuilder builder)
        {
            var conexiones = new AppDbContextEmp(builder.ConnectionString);
            string[] resultado = new string[2];
            try
            {
                using (DbCommand cm = conexiones.Database.GetDbConnection().CreateCommand())
                {
                    cm.Connection.Open();
                    cm.CommandText = sNombreSP;
                    cm.CommandType = CommandType.StoredProcedure;
                    cm.CommandTimeout = iTimeout > 0 ? iTimeout : 300;

                    AgregarParametro(cm, sVariableProc, sValor);
                    AgregarParametro(cm, sVariableProc2, dtlValor);
                    cm.Parameters.Add(new SqlParameter("@sRespuesta", SqlDbType.VarChar, -1));
                    cm.Parameters["@sRespuesta"].Direction = ParameterDirection.Output;

                    cm.ExecuteNonQuery();
                    resultado[0] = "1";
                    resultado[1] = cm.Parameters["@sRespuesta"].Value.ToString();
                }

            }
            catch (Exception ex)
            {
                resultado[0] = ex.Message;
            }
            return resultado;
        }

        public List<T> RecuperaObjetoLista<T>(string sQuery, string[] aAtributos) where T : new()
        {
            string sRes = "1";
            string sDato = "";
            List<T> lstDatos = new List<T>();
            try
            { // Establece conexion y obtiene la información.
                EstablecerConexion();
                oSQlConnection.Open();
                oSqlCommand = new SqlCommand(sQuery, oSQlConnection);
                oSqlCommand.CommandType = CommandType.Text;
                oSqlCommand.CommandTimeout = 30000;
                oSqlDataReader = oSqlCommand.ExecuteReader();
                // Recorre los registros obtenidos.
                while (oSqlDataReader.Read())
                {
                    // Crea un objeto del tipo de dato idicado en los parámetros.
                    var Data = new T();
                    // Obtiene los atributos del objeto.
                    PropertyInfo[] Properties = Data.GetType().GetProperties();
                    // Recorre las propuedades para asignarlas.
                    foreach (var p in Properties)
                    {
                        // Verifica si la propiedad esta en los atributos que se asignarán.
                        if (Array.IndexOf(aAtributos, p.Name) >= 0)
                        {
                            // Valida que el dato a asignar no es nulo, de otra forma no lo asigna. (Lo que indica un valor equivalente).
                            if (!oSqlDataReader.IsDBNull(Array.IndexOf(aAtributos, p.Name)))
                            {
                                // Se obtiene el dato a asignar en el atributo correspondiente.
                                sDato = oSqlDataReader.GetValue(Array.IndexOf(aAtributos, p.Name)).ToString();
                                // Verifica si el valor a asignar es un decimal.
                                if (p.PropertyType == typeof(decimal))
                                { // Si es un decimal lo convierte bajo la validación del tipo de valor hexadecimal(valores con e), con millones (valores con ,), y punto decimal(valores con .).
                                    sDato = oSqlDataReader.GetValue(Array.IndexOf(aAtributos, p.Name)).ToString();
                                    decimal dDato = decimal.Parse(sDato, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.AllowDecimalPoint);
                                    sDato = dDato.ToString();
                                }
                                // Se parsea el valor del reader al tipo de dato de la clase.
                                p.SetValue(Data, Convert.ChangeType(sDato, p.PropertyType, null), null);
                            }
                        }
                    }
                    lstDatos.Add(Data);
                }
            }
            catch (Exception ex)
            {
                sRes = ex.Message.ToString();
            }
            finally
            {
                oSQlConnection.Close();
                oSQlConnection.Dispose();
            }
            return lstDatos;
        }
        #endregion

        private SqlConnection oSQlConnection; // Objeto de Conexión.
        private SqlCommand oSqlCommand = null; // Linea de comandos.
        private SqlDataReader oSqlDataReader = null; // Resultados.
        public string sCadenaConexion;

        public Conexion(string sCadenaConexion)
        {
            this.sCadenaConexion = sCadenaConexion;
            this.oConexionEstadoAbierto = ConnectionState.Open;
        }

        public string EstablecerConexion()
        {
            try
            {
                oSQlConnection = new SqlConnection(Configuration[sCadenaConexion].ToString());
                return "1"; //Exito en la conexion
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return "Error al abrir la BD: " + ex.Message; //Error en la conexion
            }
        }
    }
}
