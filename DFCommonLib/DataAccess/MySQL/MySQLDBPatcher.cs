using DFCommonLib.Logger;
using System;

namespace DFCommonLib.DataAccess
{
    public class MySQLDBPatcher : IDBPatcher
    {
        private IDbConnectionFactory _connection;
        private IDFLogger<MySQLDBPatcher> _logger;
        private bool _isSuccessful;

        public MySQLDBPatcher(IDbConnectionFactory connectionFactory )
        {
            _connection = connectionFactory;
            _logger = new DFLogger<MySQLDBPatcher>();
            _isSuccessful = true;
        }

        public void Init(string patcherName)
        {
            try
            {
                // Select current database
                string schema = "no-database";
                using (var cmd = _connection.CreateCommand("SELECT database() as db") )
                {
                    cmd.AddParameter("@schema", schema);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            schema = reader["db"].ToString();
                        }
                    }
                }

                //
                // Check if the version table already exist
                //
                string sql =    "SELECT * FROM information_schema.tables " +
                                "WHERE table_schema = '@schema' " +
                                "AND table_name = 'version' " +
                                "LIMIT 1";

                using (var cmd = _connection.CreateCommand(sql) )
                {
                    cmd.AddParameter("@schema", schema);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return;
                        }
                    }
                }

                // Create the table if it does no exist
                using (var cmd = _connection.CreateCommand("create table version ( `system` varchar(50) NOT NULL DEFAULT '', `patchId` int(11) not null, `created` datetime not null, PRIMARY KEY (`patchId`))"))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception)
            {
            }
        }

        public bool Patch(string patcherName, int patchId, string sql)
        {
            // Previous was not successful. Ignore this patch
            if ( !_isSuccessful)
            {
                _logger.LogWarning( string.Format("DBPatcher: {0} Ignoring patch {1} since previous failed", patcherName, patchId) );
                return false;
            }

            if ( IsPatchExecuted( patcherName, patchId ) )
            {
                return false;
            }

            try
            {
                // Apply patch
                using (var cmd = _connection.CreateCommand(sql))
                {
                    cmd.ExecuteNonQuery();
                }

                // Update patch number
                if ( _isSuccessful )
                {
                    using (var cmd = _connection.CreateCommand("insert into version( system, patchId, created ) values( @system, @patchId, now()) "))
                    {
                        cmd.AddParameter("@system", patcherName);
                        cmd.AddParameter("@patchId", patchId);
                        cmd.ExecuteNonQuery();
                        _logger.LogInfo( string.Format("DBPatcher: {0} => Applied patch {1}", patcherName, patchId) );
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning( string.Format("DBPatcher: {0} Failed to apply patch {1} => {2}", patcherName, patchId, ex.ToString()) );
                _isSuccessful = false;
            }
            return _isSuccessful;
        }

        private bool IsPatchExecuted(string patcherName, int patchId)
        {
            using (var cmd = _connection.CreateCommand("select patchId from version where system=@system and patchId = @patchId"))
            {
                cmd.AddParameter("@system", patcherName);
                cmd.AddParameter("@patchId", patchId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Successful()
        {
            return _isSuccessful;
        }
    }
}