using DFCommonLib.DataAccess;
using DFCommonLib.Logger;

namespace DFCommonLib.DataAccess
{
    public interface IStartupDatabasePatcher
    {
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

        public virtual bool RunPatcher()
        {
            _dbPatcher.Init(PATCHER);
            _dbPatcher.Patch(PATCHER, 1, "CREATE TABLE `logtable` ( `id` int(11) NOT NULL AUTO_INCREMENT, `created` datetime NOT NULL, `loglevel` int(11) NOT NULL, `groupname` varchar(100) NOT NULL DEFAULT '', `message` varchar(1024) NOT NULL DEFAULT '', PRIMARY KEY (`id`))");
            return _dbPatcher.Successful();
        }
    }
}