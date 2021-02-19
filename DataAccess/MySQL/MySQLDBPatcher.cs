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

        public void Init()
        {
            try
            {
                using (var cmd = _connection.CreateCommand("create table version ( `patchId` int(11) not null, `created` datetime not null, PRIMARY KEY (`patchId`))"))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception)
            {
            }
        }

        public bool Patch(int patchId, string sql)
        {
            // Previous was not successful. Ignore this patch
            if ( !_isSuccessful)
            {
                _logger.LogWarning( string.Format("DBPatcher: Ignoring patch {0} since previous failed", patchId) );
                return false;
            }

            if ( IsPatchExecuted( patchId ) )
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
                    using (var cmd = _connection.CreateCommand("insert into version( patchId, created ) values( @patchId, now()) "))
                    {
                        cmd.AddParameter("@patchId", patchId);
                        cmd.ExecuteNonQuery();
                        _logger.LogInfo( string.Format("DBPatcher: Applied patch {0}", patchId) );
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogWarning( string.Format("DBPatcher: Failed to apply patch {0} => {1}", patchId, ex.ToString()) );
                _isSuccessful = false;
            }
            return _isSuccessful;
        }

        private bool IsPatchExecuted(int patchId)
        {
            using (var cmd = _connection.CreateCommand("select patchId from version where patchId = @patchId"))
            {
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