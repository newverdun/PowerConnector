using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelviSoft.Enterprise.Connectors.PowerConnector.DAL;

namespace TelviSoft.Enterprise.Connectors.PowerConnector.Logic
{
    internal class PowerLogic : IPowerLogic
    {
        #region Private Member Variables

        private IPowerDataAccess _powerData = null;

        #endregion Private Member Variables

        #region Private Properties

        /// <summary>
        /// Proveedor de datos.
        /// </summary>
        private ProviderType Provider { get; set; }

        /// <summary>
        /// Obtiene o establece la cadena utilizada para abrir la conexión.
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// Instancia de IPowerDataAccess
        /// </summary>
        public IPowerDataAccess PowerData
        {
            get
            {
                if(_powerData == null)
                {
                    _powerData = new PowerDataAccess(Provider, ConnectionString);
                }
                return _powerData;
            }
            set
            {
                _powerData = value;
            }
        }

        #endregion Private Properties

        #region Constructors

        public PowerLogic(ProviderType provider, string connectionString)
        {
            Provider = provider;
            ConnectionString = connectionString;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Inicia una transacción de base de datos.
        /// </summary>
        public void BeginTransaction()
        {
            PowerData.BeginTransaction();
        }

        /// <summary>
        /// Confirma la transacción de base de datos.
        /// </summary>
        public void Commit()
        {
            PowerData.Commit();
        }

        /// <summary>
        /// Deshace una transacción desde un estado pendiente.
        /// </summary>
        public void RollBack()
        {
            PowerData.RollBack();
        }

        /// <summary>
        /// Ejecuta una instrucción SQL en un objeto de conexión.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Número de filas afectadas.</returns>
        public int ExecuteNonQuery(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.ExecuteNonQuery(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de ExecuteNonQuery, que ejecuta una instrucción SQL en un objeto de conexión.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.ExecuteNonQueryAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devuelto por la consulta. Se omiten todas las demás columnas y filas.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Primera columna de la primera fila del conjunto de resultados.</returns>
        public object ExecuteScalar(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.ExecuteScalar(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de ExecuteScalar, que ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devuelto por la consulta. 
        /// Se omiten todas las demás columnas y filas.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<object> ExecuteScalarAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.ExecuteScalarAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en una lista de un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Lista de objeto genérico.</returns>
        public IEnumerable<T> Query<T>(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.Query<T>(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de Query, que ejecuta la consulta y retorna los resultados en una lista de un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.QueryAsync<T>(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna el resultado en un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Objeto genérico.</returns>
        public T Single<T>(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.Single<T>(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de Single, que ejecuta la consulta y retorna el resultado en un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<T> SingleAsync<T>(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.SingleAsync<T>(commandType, query, parameters);
        }

        /// <summary>
        /// Agrega o actualiza filas en DataSet.
        /// </summary>
        /// <param name="dataSet">Clase DataSet que se va a rellenar con registros y, si es necesario, con un esquema.</param>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        public void FillDataSet(ref DataSet dataSet, CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            PowerData.FillDataSet(ref dataSet, commandType, query, parameters);
        }

        /// <summary>
        /// Agrega o actualiza filas una clase DataTable.
        /// </summary>
        /// <param name="dataTable">Clase DataTable que se va a rellenar con registros y, si es necesario, con un esquema.</param>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        public void FillDataTable(ref DataTable dataTable, CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            PowerData.FillDataTable(ref dataTable, commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna un DataSet.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Clase DataSet</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.ExecuteDataSet(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna una clase DataTable.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Clase DataTable</returns>
        public DataTable ExecuteDataTable(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.ExecuteDataTable(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna un string JSON de lista de objetos.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>String JSON</returns>
        public string JsonQuery(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.JsonQuery(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de JsonQuery que, ejecuta la consulta y retorna un string JSON de lista de objetos.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<string> JsonQueryAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.JsonQueryAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna un string JSON de un objeto.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>String JSON</returns>
        public string JsonSingle(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.JsonSingle(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de JsonSingle que, ejecuta la consulta y retorna un string JSON de un objeto.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<string> JsonSingleAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.JsonSingleAsync(commandType, query, parameters);
        }

        public IEnumerable<dynamic> QueryDynamic(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.QueryDynamic(commandType, query, parameters);
        }

        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.QueryDynamicAsync(commandType, query, parameters);
        }

        public dynamic SingleDynamic(CommandType commandType, string query, params object[] parameters)
        {
            PowerData.Open();
            return PowerData.SingleDynamic(commandType, query, parameters);
        }

        public async Task<dynamic> SingleDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            await PowerData.OpenAsync();
            return await PowerData.SingleDynamicAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Inserta la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a insertar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Insert<T>(T entity) where T : class
        {
            PowerData.Insert<T>(entity);
        }

        /// <summary>
        /// Actualiza la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a actualizar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Update<T>(T entity) where T : class
        {
            PowerData.Update<T>(entity);
        }

        /// <summary>
        /// Elimina la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a eliminar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Delete<T>(T entity) where T : class
        {
            PowerData.Delete<T>(entity);
        }

        #endregion Public Properties

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_powerData != null)
                    {
                        PowerData.Close();
                        PowerData.Dispose();
                        _powerData = null;
                    }
                }

                disposedValue = true;
            }
        }

        ~PowerLogic()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
