using System;
using System.Data;
using System.Linq;
using DFCommonLib.Config;
using DFCommonLib.Logger;

namespace DFCommonLib.DataAccess
{
    public abstract class MySQLDbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionType;

        private string _connectionString;
        private IConfigurationHelper _helper;
        private IDFLogger<MySQLDbConnectionFactory> _logger;

        public MySQLDbConnectionFactory(string connectionType, IConfigurationHelper helper )
        {
            _connectionType = connectionType;
            _helper = helper;
            _logger = new DFLogger<MySQLDbConnectionFactory>();
        }

        public MySQLDbConnectionFactory(string connectionType, string connectionString )
        {
            _connectionType = connectionType;
            _connectionString = connectionString;
            _helper = null;
            _logger = new DFLogger<MySQLDbConnectionFactory>();
        }

        public IDbConnection CreateConnection()
        {
            string connectionString = GetConnectionString();
            return new MySQLDbConnection(connectionString);
        }

        public IBluDbCommand CreateCommand(string commandText)
        {
            var connection = CreateConnection();
            return new TimedMySQLDbCommand(commandText, connection as MySQLDbConnection, true);
        }

        public IBluDbCommand CreateCommand(string commandText, IDbConnection connection)
        {
            return new TimedMySQLDbCommand(commandText, connection as MySQLDbConnection, false);
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
