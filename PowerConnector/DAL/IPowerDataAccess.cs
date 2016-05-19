using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelviSoft.Enterprise.Connectors.PowerConnector.DAL
{
    internal interface IPowerDataAccess : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Abre una conexión a base de datos con la configuración que especifica ConnectionString.
        /// </summary>
        void Open();

        /// <summary>
        /// Este método implementa una versión asincrónica de Open.
        /// </summary>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task OpenAsync();

        /// <summary>
        /// Cierra la conexión con la base de datos.Es el método preferido para cerrar cualquier conexión abierta.
        /// </summary>
        void Close();

        /// <summary>
        /// Inicia una transacción de base de datos.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Confirma la transacción de base de datos.
        /// </summary>
        void Commit();

        /// <summary>
        /// Deshace una transacción desde un estado pendiente.
        /// </summary>
        void RollBack();

        /// <summary>
        /// Ejecuta una instrucción SQL en un objeto de conexión.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Número de filas afectadas.</returns>
        int ExecuteNonQuery(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de ExecuteNonQuery, que ejecuta una instrucción SQL en un objeto de conexión.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<int> ExecuteNonQueryAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devuelto por la consulta. Se omiten todas las demás columnas y filas.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Primera columna de la primera fila del conjunto de resultados.</returns>
        object ExecuteScalar(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de ExecuteScalar, que ejecuta la consulta y devuelve la primera columna de la primera fila del conjunto de resultados devuelto por la consulta. 
        /// Se omiten todas las demás columnas y filas.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<object> ExecuteScalarAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en una lista de un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Lista de objeto genérico.</returns>
        IEnumerable<T> Query<T>(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de Query, que ejecuta la consulta y retorna los resultados en una lista de un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<IEnumerable<T>> QueryAsync<T>(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna el resultado en un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Objeto genérico.</returns>
        T Single<T>(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de Single, que ejecuta la consulta y retorna el resultado en un objeto genérico.
        /// </summary>
        /// <typeparam name="T">Objeto genérico, en el nombre de sus propiedades deben coincidir con los nombres de las columnas de la consulta.</typeparam>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<T> SingleAsync<T>(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Agrega o actualiza filas en DataSet.
        /// </summary>
        /// <param name="dataSet">Clase DataSet que se va a rellenar con registros y, si es necesario, con un esquema.</param>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        void FillDataSet(ref DataSet dataSet, CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Agrega o actualiza filas una clase DataTable.
        /// </summary>
        /// <param name="dataTable">Clase DataTable que se va a rellenar con registros y, si es necesario, con un esquema.</param>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        void FillDataTable(ref DataTable dataTable, CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna un DataSet.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Clase DataSet</returns>
        DataSet ExecuteDataSet(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna una clase DataTable.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Clase DataTable</returns>
        DataTable ExecuteDataTable(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna un string JSON de lista de objetos.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>String JSON</returns>
        string JsonQuery(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de JsonQuery que, ejecuta la consulta y retorna un string JSON de lista de objetos.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<string> JsonQueryAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna un string JSON de un objeto.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>String JSON</returns>
        string JsonSingle(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de JsonSingle que, ejecuta la consulta y retorna un string JSON de un objeto.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<string> JsonSingleAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en una lista de un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Lista de objeto dinámico.</returns>
        IEnumerable<dynamic> QueryDynamic(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de QueryDynamic que, ejecuta la consulta y retorna los resultados en una lista de un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<IEnumerable<dynamic>> QueryDynamicAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Ejecuta la consulta y retorna los resultados en un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Objeto dinámico.</returns>
        dynamic SingleDynamic(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Una versión asincrónica de SingleDynamic que, ejecuta la consulta y retorna los resultados en un objeto dinámico.
        /// </summary>
        /// <param name="commandType">Especifica cómo la instrucción SQL se interpreta.</param>
        /// <param name="query">Establece el texto de comando para ejecutar en el origen de datos.</param>
        /// <param name="parameters">Valores para los parámetros de la consulta (si aplica).</param>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        Task<dynamic> SingleDynamicAsync(CommandType commandType, string query, params object[] parameters);

        /// <summary>
        /// Inserta la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a insertar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        void Insert<T>(T entity) where T : class;

        /// <summary>
        /// Actualiza la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a actualizar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        void Update<T>(T entity) where T : class;

        /// <summary>
        /// Elimina la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a eliminar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        void Delete<T>(T entity) where T : class;

        #endregion Public Properties
    }
}
