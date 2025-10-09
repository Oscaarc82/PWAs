using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using PWAs.Models.Conexion;

namespace PWAs.Sistema
{
    public class storedProcedure
    {
        private SqlConnection conexion;
        SqlCommand comando = null;
        SqlDataReader resultados = null;
        private string conn;
        private readonly IConfiguration Configuration;

        public string Conn
        {
            get { return conn; }
            set { conn = value; }
        }
        public storedProcedure(string conn, IConfiguration config)
        {
            this.conn = conn;
            Configuration = config;
        }

        public void establecerConexion()
        {
            conexion = new SqlConnection(conn);

        }

        public bool logNParte(string idNParte, string numParte, string descripcionNP, string idUMC, string idFraccion20, string idDescripcionG, string idFraccion12, string idSistema, string idUsuario, DateTime fecha)
        {
            bool bAlta;
            string query = "";                        
            string sDBEMP = Configuration["BaseEMP"];

            try
            {
                if (idFraccion12 != null || idFraccion12 != "")
                {
                    query = "Insert Into " + sDBEMP + ".dbo.tbCambiosMaestroPartes (iIdNParte, sNumParte, sDescripcionComercial, iIdUMC, iIdFraccion, sDescripcionF, iIdDescripcionG, iIdFraccion2012, sDescripcionCambio, iIdSistema, iIdUsuario, sFechaCambio)" +
                                        " Values (" + idNParte + ", '" + numParte + "', '" + descripcionNP + "', '" + idUMC + "', '" + idFraccion20 + "', '" + descripcionNP + "', '" + idDescripcionG + "', '" + idFraccion12 + "', 'Insert', '" + idSistema + "', '" + idUsuario + "', '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                }
                else
                {
                    query = "Insert Into " + sDBEMP + ".dbo.tbCambiosMaestroPartes (iIdNParte, sNumParte, sDescripcionComercial, iIdUMC, iIdFraccion, sDescripcionF, iIdDescripcionG, sDescripcionCambio, iIdSistema, iIdUsuario, sFechaCambio)" +
                                        " Values (" + idNParte + ", '" + numParte + "', '" + descripcionNP + "', '" + idUMC + "', '" + idFraccion20 + "', '" + descripcionNP + "', '" + idDescripcionG + "', 'Insert', '" + idSistema + "', '" + idUsuario + "', '" + fecha.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                }

                bAlta = ejecutaSQL(query);
            }
            catch (Exception e)
            {
                bAlta = false;
            }
            return bAlta;
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

        public string actualizarMPR(string numProducto, string numParte, string descripcionF, int idDescripcionG, string fraccion, int idTratado, int idTipoBien, int peso, int idUM, int idRegimen, int idProducto)
        {

            string result = "";
            establecerConexion();

            comando = new SqlCommand("spModMPR", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idProducto", SqlDbType.Int).Value = idProducto;
            comando.Parameters.Add("@numProducto", SqlDbType.VarChar).Value = numProducto;
            comando.Parameters.Add("@numParteC", SqlDbType.VarChar).Value = numParte;
            comando.Parameters.Add("@descripcionF", SqlDbType.VarChar).Value = descripcionF;
            comando.Parameters.Add("@idDescripcionG", SqlDbType.Int).Value = idDescripcionG;
            comando.Parameters.Add("@fraccion", SqlDbType.VarChar).Value = fraccion;
            comando.Parameters.Add("@idTratado", SqlDbType.Int).Value = idTratado;
            comando.Parameters.Add("@idTipoBien", SqlDbType.Int).Value = idTipoBien;
            comando.Parameters.Add("@peso", SqlDbType.Int).Value = peso;
            comando.Parameters.Add("@idUM", SqlDbType.Int).Value = idUM;
            comando.Parameters.Add("@idRegimen", SqlDbType.Int).Value = idRegimen;
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
                return result;
            }
        }
        public string eliminaMPR(int idProducto)
        {
            string result = "";
            establecerConexion();

            comando = new SqlCommand("spEliminarMPR", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idProducto", SqlDbType.Int).Value = idProducto;


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
                return result;
            }
        }
        public string altaMPR(string numProducto, string numParte, string descripcionF, int idDescripcionG, string fraccion, int idTratado, int idTipoBien, int peso, int idUM, int idRegimen)
        {
            string result = "";
            establecerConexion();

            comando = new SqlCommand("spAltaMPR", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@numProducto", SqlDbType.VarChar).Value = numProducto;
            comando.Parameters.Add("@numParteC", SqlDbType.VarChar).Value = numParte;
            comando.Parameters.Add("@descripcionF", SqlDbType.VarChar).Value = descripcionF;
            comando.Parameters.Add("@idDescripcionG", SqlDbType.Int).Value = idDescripcionG;
            comando.Parameters.Add("@fraccion", SqlDbType.VarChar).Value = fraccion;
            comando.Parameters.Add("@idTratado", SqlDbType.Int).Value = idTratado;
            comando.Parameters.Add("@idTipoBien", SqlDbType.Int).Value = idTipoBien;
            comando.Parameters.Add("@peso", SqlDbType.Int).Value = peso;
            comando.Parameters.Add("@idUM", SqlDbType.Int).Value = idUM;
            comando.Parameters.Add("@idRegimen", SqlDbType.Int).Value = idRegimen;


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
                return result;
            }
        }
        public string actualizarMP(string numParte, float precio, string descripcionF, int idDescripcionG, int idTratado, int idTipoBien, float peso, int idUM, int idRegimen, int idPais, int idPrefArancelaria, string identificadorPartida, int tasa, string pruebaOrigen, int idMaestroParte, string fraccion)
        {

            string result = "";
            establecerConexion();

            comando = new SqlCommand("spModMP", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@numParte", SqlDbType.VarChar).Value = numParte;
            comando.Parameters.Add("@precio", SqlDbType.Float).Value = precio;
            comando.Parameters.Add("@descripcionF", SqlDbType.VarChar).Value = descripcionF;
            comando.Parameters.Add("@idDescripcionG", SqlDbType.Int).Value = idDescripcionG;
            comando.Parameters.Add("@idTratado", SqlDbType.Int).Value = idTratado;
            comando.Parameters.Add("@peso", SqlDbType.Float).Value = peso;
            comando.Parameters.Add("@idUM", SqlDbType.Int).Value = idUM;
            comando.Parameters.Add("@idRegimen", SqlDbType.Int).Value = idRegimen;
            comando.Parameters.Add("@idPais", SqlDbType.Int).Value = idPais;
            comando.Parameters.Add("@idPrefArancelaria", SqlDbType.Int).Value = idPrefArancelaria;
            comando.Parameters.Add("@identificadorPartida", SqlDbType.VarChar).Value = identificadorPartida;
            comando.Parameters.Add("@tasa", SqlDbType.Int).Value = tasa;
            comando.Parameters.Add("@pruebaOrigen", SqlDbType.VarChar).Value = pruebaOrigen;
            comando.Parameters.Add("@idMaestroParte", SqlDbType.Int).Value = idMaestroParte;

            comando.Parameters.Add("@fraccion", SqlDbType.VarChar).Value = fraccion;
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
                return result;
            }
        }
        public string eliminaMP(int idNParte)
        {
            string result = "";
            establecerConexion();

            comando = new SqlCommand("spEliminarMP", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@idNParte", SqlDbType.Int).Value = idNParte;


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
                return result;
            }
        }
        public string altaMP(string numParte, int umc, string fraccion, float precio, int idPais, int idPrefArrancelaria, string identificadorPartida, int tasa, float peso, string pruebaOrigen, int idTipoBien, int idRegimen, string descripcionF, int idDescripcionG, int idTratado)
        {
            string result = "";
            establecerConexion();

            comando = new SqlCommand("spAltaMP", conexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.Add("@numParte", SqlDbType.VarChar).Value = numParte;
            comando.Parameters.Add("@umc", SqlDbType.Int).Value = umc;
            comando.Parameters.Add("@fraccion", SqlDbType.VarChar).Value = fraccion;
            comando.Parameters.Add("@precio", SqlDbType.Float).Value = precio;
            comando.Parameters.Add("@idPais", SqlDbType.Int).Value = idPais;
            comando.Parameters.Add("@idPrefArrancelaria", SqlDbType.Int).Value = idPrefArrancelaria;
            comando.Parameters.Add("@identificadorPartida", SqlDbType.VarChar).Value = identificadorPartida;
            comando.Parameters.Add("@tasa", SqlDbType.Int).Value = tasa;
            comando.Parameters.Add("@peso", SqlDbType.Float).Value = peso;
            comando.Parameters.Add("@pruebaOrigen", SqlDbType.VarChar).Value = pruebaOrigen;
            comando.Parameters.Add("@idTipoBien", SqlDbType.Int).Value = idTipoBien;
            comando.Parameters.Add("@idRegimen", SqlDbType.Int).Value = idRegimen;
            comando.Parameters.Add("@descripcionF", SqlDbType.VarChar).Value = descripcionF;
            comando.Parameters.Add("@idDescripcionG", SqlDbType.Int).Value = idDescripcionG;
            comando.Parameters.Add("@idTratado", SqlDbType.Int).Value = idTratado;

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
                return result;
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
            List<string> resultado = new List<string>();
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
                return null;
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }

        }

        public List<Dictionary<string, string>> recuperaRegistroslst(string query)
        {
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
                return null;
            }
            finally
            {
                conexion.Dispose();
                conexion.Close();
            }
        }

        public string recuperaValor(string query)
        {
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

        #region OBTENER OBJETOS
        #region OBTENER OBJETO CON LISTA DE ATRIBUTOS
        /// <summary>
        /// Permite recuperar un objeto y asignar unicamente los atributos necesario al pasar la lista de los atributos
        /// </summary>
        /// <typeparam name="T">Tipo de Objeto a obtener.</typeparam>
        /// <param name="sQuery">Consulta para obtener los datos de los objetos.</param>
        /// <param name="lstAtributos">Lista de atributos a asiganar.</param>
        /// <returns>Objeto de tipo correspondiente.</returns>
        public T ObtenerObjeto<T>(string sQuery, List<string> lstAtributos) where T : new()
        {
            T oT = new T();
            establecerConexion();
            try
            {
                comando = new SqlCommand(sQuery, conexion);
                comando.CommandType = CommandType.Text;
                comando.CommandTimeout = 30000;
                conexion.Open();
                resultados = comando.ExecuteReader();

                while (resultados.Read())
                {
                    var Data = new T();
                    PropertyInfo[] Properties = Data.GetType().GetProperties();
                    foreach (var p in Properties)
                    {
                        if (lstAtributos.Contains(p.Name.ToString()))
                        {
                            p.SetValue(Data, resultados.GetValue(resultados.GetOrdinal(p.Name)), null);
                        }
                    }
                    oT = Data;
                }

                this.conexion.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                this.conexion.Dispose();
                this.conexion.Close();
            }

            return oT;
        }
        #endregion
        #endregion
        #region OBTENER LISTA DE OBJETOS PASANDO ATRIBUTOS
        /// <summary>
        /// Permite recuperar una lista de objetos asignando unicamente los atributos necesarios.
        /// </summary>
        /// <typeparam name="T">Tipo de dato del objeto a regresar.</typeparam>
        /// <param name="sQuery">Consulta para llenar la lista de objetos.</param>
        /// <param name="aAtributos">Arreglo de atributos a asignar.</param>
        /// <returns>Lista de objetos con los datos obtenidos.</returns>
        public List<T> RecuperaObjetoLista<T>(string sQuery, string[] aAtributos) where T : new()
        {
            string sRes = "1";
            string sDato = "";
            List<T> lstDatos = new List<T>();
            try
            { // Establece conexion y obtiene la información.
                establecerConexion();
                conexion.Open();
                comando = new SqlCommand(sQuery, conexion);
                comando.CommandType = CommandType.Text;
                comando.CommandTimeout = 30000;
                resultados = comando.ExecuteReader();
                // Recorre los registros obtenidos.
                while (resultados.Read())
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
                            if (!resultados.IsDBNull(Array.IndexOf(aAtributos, p.Name)))
                            {
                                // Se obtiene el dato a asignar en el atributo correspondiente.
                                sDato = resultados.GetValue(Array.IndexOf(aAtributos, p.Name)).ToString();
                                // Verifica si el valor a asignar es un decimal.
                                if (p.PropertyType == typeof(decimal))
                                { // Si es un decimal lo convierte bajo la validación del tipo de valor hexadecimal(valores con e), con millones (valores con ,), y punto decimal(valores con .).
                                    sDato = resultados.GetValue(Array.IndexOf(aAtributos, p.Name)).ToString();
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
                conexion.Close();
                conexion.Dispose();
            }
            return lstDatos;
        }
        #endregion
        #endregion
    }
}
