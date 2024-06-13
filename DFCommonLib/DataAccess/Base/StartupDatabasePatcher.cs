using System;
using System.Data.Common;
using System.Threading;
using DFCommonLib.DataAccess;
using DFCommonLib.Logger;

namespace DFCommonLib.DataAccess
{
    public interface IStartupDatabasePatcher
    {
        void WaitForConnection();
        void WaitForConnection(int timeoutSeconds);
        bool RunPatcher();
    }

    public class StartupDatabasePatcher : IStartupDatabasePatcher
    {
        private static string PATCHER = "StartupDatabasePatcher";

        private IDBPatcher _dbPatcher;

        public StartupDatabasePatcher(IDBPatcher dbPatcher)
        {
            _dbPatcher = dbPatcher;
        }

        public void WaitForConnection()
        {
            WaitForConnection(60);
        }

        public void WaitForConnection(int timeoutSeconds)
        {
            while( timeoutSeconds > 0.0f )
            {
                if ( _dbPatcher.IsConnected() )
                {
                    return;
                }

                // Sleep one second
                Thread.Sleep(1000);
                timeoutSeconds -= 1;
            }
        }

        public virtual bool RunPatcher()
        {
            _dbPatcher.Init(PATCHER);
            _dbPatcher.Patch(PATCHER, 1, "CREATE TABLE `logtable` ( `id` int(11) NOT NULL AUTO_INCREMENT, `created` datetime NOT NULL, `loglevel` int(11) NOT NULL, `groupname` varchar(100) NOT NULL DEFAULT '', `message` varchar(1024) NOT NULL DEFAULT '', PRIMARY KEY (`id`))");
            return _dbPatcher.Successful();
        }
    }
}