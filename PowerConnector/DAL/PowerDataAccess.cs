using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TelviSoft.Enterprise.Connectors.PowerConnector.Factory;
using Dapper;
using DapperExtensions;
using System.Dynamic;
using Newtonsoft.Json;

namespace TelviSoft.Enterprise.Connectors.PowerConnector.DAL
{
    internal class PowerDataAccess : IPowerDataAccess
    {
        #region Constants

        private const string ExceptionMessageParameterMatchFailure = "El número de parámetros no coincide con el número de valores de procedimiento almacenado.";

        #endregion Constants

        #region Private Member Variables

        private DbProviderFactory _dbFactory = null;
        private DbConnection _connection = null;

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
        /// Clase base para una transacción.
        /// </summary>
        private DbTransaction Transaction { get; set; }

        /// <summary>
        /// Representa una conexión a una base de datos.
        /// </summary>
        private DbConnection Connection
        {
            get
            {
                if(_connection == null)
                {
                    _connection = _dbFactory.CreateConnection();
                    _connection.ConnectionString = ConnectionString;
                }
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        #endregion Private Properties

        #region Private Methods

        /// <summary>
        /// Construye los parámetros y valores contenidos en el objeto DbCommand para usarlos en la libreria Dapper.
        /// </summary>
        /// <param name="command">Objeto DbCommand que contiene los parámetros.</param>
        /// <returns>Objeto dinámico.</returns>
        private object BuildDapperParameters(DbCommand command)
        {
            dynamic expando = new ExpandoObject();

            foreach (DbParameter item in command.Parameters)
            {
                (expando as IDictionary<string, object>).Add(item.ParameterName.Replace("@", ""), item.Value);
            }

            return expando;
        }

        /// <summary>
        /// Establece un valor de parámetro.
        /// </summary>
        /// <param name="command">Objeto DbCommand que contiene el parámetro.</param>
        /// <param name="parameterName">Nombre del parámetro.</param>
        /// <param name="value">Valor del parámetro.</param>
        private void SetParameterValue(DbCommand command, string parameterName, object value)
        {
            if (command == null) throw new ArgumentNullException("command");

            command.Parameters[parameterName].Value = value ?? DBNull.Value;
        }

        /// <summary>
        /// Asigna valores a los parámetros en el objeto DbCommand
        /// </summary>
        /// <param name="command">Objeto DbCommand que contiene los parámetros.</param>
        /// <param name="values">Valores de parámetros.</param>
        private void AssignParameterValues(DbCommand command, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                DbParameter parameter = command.Parameters[i];
                SetParameterValue(command, parameter.ParameterName, values[i]);
            }
        }

        /// <summary>
        /// Determina si el número de parámetros en el comando coincide con el conjunto de valores de parámetros.
        /// </summary>
        /// <param name="command">Objeto <see cref="DbCommand"/> que contiene los parámetros.</param>
        /// <param name="values">Arreglo de valores de los parámetros</param>
        /// <returns><see langword="true"/> si el número de parámetros y valores coinciden; de otra manera, <see langword="false"/>.</returns>
        protected bool SameNumberOfParametersAndValues(DbCommand command, object[] values)
        {
            int numberOfParametersToStoredProcedure = command.Parameters.Count;
            int numberOfValuesProvidedForStoredProcedure = values.Length;

            if(command.GetType() == typeof(System.Data.SqlClient.SqlCommand) && (numberOfParametersToStoredProcedure - numberOfValuesProvidedForStoredProcedure) == 1)
            {
                command.Parameters["@RETURN_VALUE"].Value = 0;
                return true;
            }

            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <summary>
        /// <para>Recupera información de parámetro del procedimiento almacenado especificado en DbCommand y rellena la colección de Parameters del objeto DbCommand especificado.</para>
        /// </summary>
        /// <param name="command">Objeto DbCommand que se le asignarán valores.</param>
        /// <param name="parameterValues">Los valores de los parámetros que serán asignados al objeto DbCommand.</param>
        private void AssignParameters(DbCommand command, object[] parameterValues)
        {
            if (SameNumberOfParametersAndValues(command, parameterValues) == false)
            {
                throw new InvalidOperationException(ExceptionMessageParameterMatchFailure);
            }

            AssignParameterValues(command, parameterValues);
        }

        /// <summary>
        /// Obtiene los nombres de los parámetros en la consulta SQL
        /// </summary>
        /// <param name="commandText">Consulta SQL</param>
        /// <returns>Lista de nombre de parámetros</returns>
        private List<string> GetParameterNames(string commandText)
        {
            string sPattern = @"(?<!@)@\w{1,}";

            return Regex.Matches(commandText, sPattern, RegexOptions.IgnoreCase).Cast<Match>().Select(m => m.Value).ToList();
        }

        /// <summary>
        /// Crear parámetros DbCommand cuando su propiedad CommandType es texto
        /// </summary>
        /// <param name="command">Objeto DbCommand que se va a crear los parámetros.</param>
        private void CreateNormalDbCommandParameters(DbCommand command)
        {
            List<string> parametersNames = GetParameterNames(command.CommandText);
            foreach (string parameterName in parametersNames)
            {
                DbParameter parameter = _dbFactory.CreateParameter();
                parameter.ParameterName = parameterName;
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Crea y asigna automáticamente los parámetros al objeto DbCommand.
        /// </summary>
        /// <param name="command">Objeto DbCommand que se va a crear los parámetros.</param>
        private void CreateAutomaticDbCommandParameters(DbCommand command)
        {
            switch (command.CommandType)
            {
                case CommandType.StoredProcedure:
                    DbFactory.DeriveParameters(Provider, command);
                    //command.Parameters.RemoveAt("@RETURN_VALUE");
                    break;
                case CommandType.Text:
                    CreateNormalDbCommandParameters(command);
                    break;
            }
        }

        /// <summary>
        /// Agrega una serie de parámetros a un objeto DbCommand.
        /// </summary>
        /// <param name="command">Objeto DbCommand.</param>
        /// <param name="commandParameters">Lista de parámetros a agregar</param>
        private void AttachParameters(DbCommand command, object[] commandParameters)
        {
            CreateAutomaticDbCommandParameters(command);
            AssignParameters(command, commandParameters);
        }

        /// <summary>
        /// Preparar el objeto DbCommand configurando sus diferentes propiedades.
        /// </summary>
        /// <param name="command">Objeto DbCommand</param>
        /// <param name="commandType">CommandType que se va a usar</param>
        /// <param name="commandText">Consulta SQL que se va a ejecutar</param>
        /// <param name="commandParameters">Parámetros de la consulta.</param>
        private void PrepareCommand(DbCommand command, CommandType commandType, string commandText, object[] commandParameters)
        {
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (commandParameters != null && commandParameters.Length > 0)
            {
                AttachParameters(command, commandParameters);
            }
        }

        #endregion Private Methods

        #region Constructors

        public PowerDataAccess(ProviderType provider, string connectionString)
        {
            Provider = provider;
            _dbFactory = DbFactory.GetProvider(Provider);
            ConnectionString = connectionString;
        }

        #endregion Constructors

        #region Public Properties

        /// <summary>
        /// Cierra la conexión con la base de datos.Es el método preferido para cerrar cualquier conexión abierta.
        /// </summary>
        public void Close()
        {
            if(Connection.State != ConnectionState.Closed)
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Abre una conexión a base de datos con la configuración que especifica ConnectionString.
        /// </summary>
        public void Open()
        {
            if(Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }
        }

        /// <summary>
        /// Este método implementa una versión asincrónica de Open.
        /// </summary>
        /// <returns>Tarea que representa la operación asincrónica.</returns>
        public async Task OpenAsync()
        {
            if (Connection.State != ConnectionState.Open)
            {
                await Connection.OpenAsync();
            }
        }

        /// <summary>
        /// Inicia una transacción de base de datos.
        /// </summary>
        public void BeginTransaction()
        {
            if(Transaction == null)
            {
                Open();
                Transaction = Connection.BeginTransaction();
            }
        }

        /// <summary>
        /// Confirma la transacción de base de datos.
        /// </summary>
        public void Commit()
        {
            if (Transaction != null)
            {
                Transaction.Commit();
            }
        }

        /// <summary>
        /// Deshace una transacción desde un estado pendiente.
        /// </summary>
        public void RollBack()
        {
            if (Transaction != null)
            {
                Transaction.Rollback();
            }
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
            int affectedRows = 0;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                affectedRows = command.ExecuteNonQuery();
            }

            return affectedRows;
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
            int affectedRows = 0;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                affectedRows = await command.ExecuteNonQueryAsync();
            }

            return affectedRows;
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
            object result = null;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                result = command.ExecuteScalar();
            }

            return result;
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
            object result = null;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                result = await command.ExecuteScalarAsync();
            }

            return result;
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return Connection.Query<T>(query, BuildDapperParameters(command), Transaction);
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return await Connection.QueryAsync<T>(query, BuildDapperParameters(command), Transaction);
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return Connection.QuerySingleOrDefault<T>(query, BuildDapperParameters(command), Transaction);
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return await Connection.QuerySingleOrDefaultAsync<T>(query, BuildDapperParameters(command), Transaction);
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                using (DbDataAdapter adapter = _dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    adapter.FillSchema(dataSet, SchemaType.Source);
                    adapter.Fill(dataSet);
                }
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                using (DbDataAdapter adapter = _dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    adapter.FillSchema(dataTable, SchemaType.Source);
                    adapter.Fill(dataTable);
                }
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                using (DbDataAdapter adapter = _dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    adapter.FillSchema(dataSet, SchemaType.Source);
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
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
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                command.Transaction = Transaction;
                using (DbDataAdapter adapter = _dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    DataTable dataTable = new DataTable();
                    adapter.FillSchema(dataTable, SchemaType.Source);
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
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
            string json = string.Empty;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                
                IEnumerable<dynamic> result = Connection.Query(query, BuildDapperParameters(command), Transaction);
                if (result.Count() > 0)
                {
                    json = JsonConvert.SerializeObject(result);
                }
            }

            return json;
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
            string json = string.Empty;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);

                IEnumerable<dynamic> result = await Connection.QueryAsync(query, BuildDapperParameters(command), Transaction);
                if (result.Count() > 0)
                {
                    json = JsonConvert.SerializeObject(result);
                }
            }

            return json;
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
            string json = string.Empty;
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);

                dynamic result = Connection.QuerySingleOrDefault(query, BuildDapperParameters(command), Transaction);
                if (result != null)
                {
                    json = JsonConvert.SerializeObject(result);
                }
            }

            return json;
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
            string json = string.Empty;

            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);

                object result = await Connection.QuerySingleOrDefaultAsync(typeof(object), query, BuildDapperParameters(command), Transaction);
                if (result != null)
                {
                    json = JsonConvert.SerializeObject(result);
                }
            }

            return json;
        }

        public IEnumerable<dynamic> QueryDynamic(CommandType commandType, string query, params object[] parameters)
        {
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return Connection.Query(query, BuildDapperParameters(command), Transaction);
            }
        }

        public async Task<IEnumerable<dynamic>> QueryDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return await Connection.QueryAsync(query, BuildDapperParameters(command), Transaction);
            }
        }

        public dynamic SingleDynamic(CommandType commandType, string query, params object[] parameters)
        {
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return Connection.QueryFirstOrDefault(query, BuildDapperParameters(command), Transaction);
            }
        }

        public async Task<dynamic> SingleDynamicAsync(CommandType commandType, string query, params object[] parameters)
        {
            using (DbCommand command = Connection.CreateCommand())
            {
                PrepareCommand(command, commandType, query, parameters);
                return await Connection.QueryFirstOrDefaultAsync(typeof(object), query, BuildDapperParameters(command), Transaction);
            }
        }

        /// <summary>
        /// Inserta la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a insertar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Insert<T>(T entity) where T : class
        {
            Connection.Insert<T>(entity, Transaction);
        }

        /// <summary>
        /// Actualiza la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a actualizar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Update<T>(T entity) where T : class
        {
            Connection.Update<T>(entity, Transaction);
        }

        /// <summary>
        /// Elimina la entidad indicada en la base de datos.
        /// </summary>
        /// <typeparam name="T">Entidad</typeparam>
        /// <param name="entity">Entidad que se va a eliminar, sus propiedades deben coincidir con la tabla a realizar la acción.</param>
        public void Delete<T>(T entity) where T : class
        {
            Connection.Delete<T>(entity, Transaction);
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
                    this.Close();
                    this.Connection.Dispose();
                    this._connection = null;
                    if(this.Transaction != null)
                    {
                        this.Transaction.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        
        ~PowerDataAccess()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
