using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelviSoft.Enterprise.Connectors.PowerConnector.Factory
{
    internal class DbFactory
    {
        private const string sqliteDeriveParametersMsgError = "SQLite no tiene soporte para DeriveParameters";
        private const string sqliteSpError = "SQLite no permite procedimientos almacenados";

        public static DbProviderFactory GetProvider(ProviderType provider)
        {
            switch(provider)
            {
                case ProviderType.SqlServer:
                    return SqlClientFactory.Instance;
                case ProviderType.MySQL:
                    return MySqlClientFactory.Instance;
                case ProviderType.SQLite:
                    return SQLiteFactory.Instance;
                default:
                    return null;
            }
        }

        public static void DeriveParameters(ProviderType provider, DbCommand command)
        {
            switch(provider)
            {
                case ProviderType.SqlServer:
                    SqlCommandBuilder.DeriveParameters(command as SqlCommand);
                    break;
                case ProviderType.MySQL:
                    MySqlCommandBuilder.DeriveParameters(command as MySqlCommand);
                    break;
                case ProviderType.SQLite:
                    throw new InvalidOperationException(sqliteSpError, new InvalidOperationException(sqliteDeriveParametersMsgError));
            }
        }
    }
}
