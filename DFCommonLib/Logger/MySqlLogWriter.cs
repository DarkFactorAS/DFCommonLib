using System;
using DFCommonLib.DataAccess;
using DFCommonLib.Utils;

namespace DFCommonLib.Logger
{
    public class MySqlLogWriter : ILogOutputWriter
    {
        IDbConnectionFactory _connection;

        static int MESSAGE_LENGTH = 1024;

        public MySqlLogWriter(IDbConnectionFactory connection)
        {
            _connection = connection;
        }
        public string GetName()
        {
            return "MySqlLogWriter";
        }

        public void LogMessage(DFLogLevel logLevel, string group, string message, int errorId)
        {
        }

        public int LogMessage(DFLogLevel logLevel, string group, string message)
        {
            if ( _connection == null )
            {
                return 0;
            }

            var sql = @"insert into logtable (id,created, loglevel, groupname, message) values(0,sysdate(), @loglevel,@group,@message)";
            using (var command = _connection.CreateCommand(sql))
            {
                command.AddParameter("@loglevel", (int) logLevel);
                command.AddParameter("@group", group);
                command.AddParameter("@message", DFCommonUtil.CapString(message, MESSAGE_LENGTH) );
                command.ExecuteNonQuery();
            }
            return GetLastId();
        }

        private int GetLastId()
        {
            var sql = "SELECT LAST_INSERT_ID() as ID";
            using (var cmd = _connection.CreateCommand(sql))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return Convert.ToInt32(reader["id"]);
                    }
                }
                return 0;
            }
        }
    }
}
