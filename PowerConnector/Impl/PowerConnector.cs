using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelviSoft.Enterprise.Connectors.PowerConnector.Logic;

namespace TelviSoft.Enterprise.Connectors.PowerConnector.Impl
{
    /// <summary>
    /// Permite realizar eficazmente de forma masiva o individual operaciones a una fuente de datos.
    /// </summary>
    public class PowerConnector : IPowerConnector
    {
        #region Private Member Variables

        private IPowerLogic _powerLogic = null;

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
        /// Instancia de IPowerLogic
        /// </summary>
        private IPowerLogic PowerLogic
        {
            get
            {
                if (_powerLogic == null)
                {
                    _powerLogic = new PowerLogic(Provider, ConnectionString);
                }
                return _powerLogic;
            }
            set
            {
                _powerLogic = value;
            }
        }

        #endregion Private Properties

        #region Constructors

        /// <summary>
        /// Inicializa una nueva instancia de la clase PowerConnector, dada un tipo de proveedor de datos 
        /// y una cadena que contiene la cadena de conexión.
        /// </summary>
        /// <param name="provider">Establece el tipo de proveedor para abrir la conexión.</param>
        /// <param name="connectionString">Establece la cadena que se utiliza para abrir la conexión.</param>
        public PowerConnector(ProviderType provider, string connectionString)
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
            PowerLogic.BeginTransaction();
        }

        /// <summary>
        /// Confirma la transacción de base de datos.
        /// </summary>
        public void Commit()
        {
            PowerLogic.Commit();
        }

        /// <summary>
        /// Deshace una transacción desde un estado pendiente.
        /// </summary>
        public void RollBack()
        {
            PowerLogic.RollBack();
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
            
            return PowerLogic.ExecuteNonQuery(commandType, query, parameters);
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
            
            return await PowerLogic.ExecuteNonQueryAsync(commandType, query, parameters);
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
            
            return PowerLogic.ExecuteScalar(commandType, query, parameters);
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
            
            return await PowerLogic.ExecuteScalarAsync(commandType, query, parameters);
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
            
            return PowerLogic.Query<T>(commandType, query, parameters);
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
            
            return await PowerLogic.QueryAsync<T>(commandType, query, parameters);
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
            
            return PowerLogic.Single<T>(commandType, query, parameters);
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
            
            return await PowerLogic.SingleAsync<T>(commandType, query, parameters);
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
            
            PowerLogic.FillDataSet(ref dataSet, commandType, query, parameters);
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
            
            PowerLogic.FillDataTable(ref dataTable, commandType, query, parameters);
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
            
            return PowerLogic.ExecuteDataSet(commandType, query, parameters);
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
            
            return PowerLogic.ExecuteDataTable(commandType, query, parameters);
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
            
            return PowerLogic.JsonQuery(commandType, query, parameters);
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
            
            return await PowerLogic.JsonQueryAsync(commandType, query, parameters);
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
            
            return PowerLogic.JsonSingle(commandType, query, parameters);
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
            return await PowerLogic.JsonSingleAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en una lista de un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Lista de objeto dinámico.</returns>
        public IEnumerable<dynamic> QueryDynamic(CommandType commandType, string query, params object[] parameters)
        {
            
            return PowerLogic.QueryDynamic(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de QueryDynamic que, ejecuta la consulta y retorna los resultados en una lista de un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            
            return await PowerLogic.QueryDynamicAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Objeto dinámico.</returns>
        public dynamic SingleDynamic(CommandType commandType, string query, params object[] parameters)
        {
            
            return PowerLogic.SingleDynamic(commandType, query, parameters);
        }

        /// <summary>
        /// Una versión asincrónica de SingleDynamic que, ejecuta la consulta y retorna los resultados en un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task<dynamic> SingleDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            return await PowerLogic.SingleDynamicAsync(commandType, query, parameters);
        }

        /// <summary>
        /// Inserta la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a insertar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Insert<T>(T entity) where T : class
        {
            PowerLogic.Insert<T>(entity);
        }

        /// <summary>
        /// Actualiza la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a actualizar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Update<T>(T entity) where T : class
        {
            PowerLogic.Update<T>(entity);
        }

        /// <summary>
        /// Elimina la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a eliminar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Delete<T>(T entity) where T : class
        {
            PowerLogic.Delete<T>(entity);
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
                    if (_powerLogic != null)
                    {
                        PowerLogic.Dispose();
                        _powerLogic = null;
                    }
                }

                disposedValue = true;
            }
        }

        ~PowerConnector()
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
