using System;
using System.Data;
using DFCommonLib.Config;

namespace DFCommonLib.DataAccess
{
    public abstract class OracleDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionType;
        private string _connectionString;
        private IConfigurationHelper _helper;

        public OracleDbConnectionFactory(string connectionType, IConfigurationHelper helper)
        {
            _connectionType = connectionType;
            _helper = helper;
        }

        public IDbConnection CreateConnection()
        {
            return new OracleDbConnection(GetConnectionString());
        }

        public IBluDbCommand CreateCommand(string commandText)
        {
            var connection = CreateConnection();
            return new TimedOracleDbCommand(commandText, connection as OracleDbConnection, true);
        }

        public IBluDbCommand CreateCommand(string commandText, IDbConnection connection)
        {
            return new TimedOracleDbCommand(commandText, connection as OracleDbConnection, false);
        }

        private string GetConnectionString()
        {
            if (_connectionString == null)
            {
                var configDbConnection = _helper.Settings.DatabaseConnection;
                if (configDbConnection == null)
                {
                    throw new Exception("DB connection returned NULL, make sure customer has a connection in the config");
                }

                _connectionString = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};SslMode={5};",
                    configDbConnection.Server,
                    configDbConnection.Port,
                    configDbConnection.Database,
                    configDbConnection.Username,
                    configDbConnection.Password,
                    configDbConnection.SslMode);
            }
            return _connectionString;
        }
    }

}
