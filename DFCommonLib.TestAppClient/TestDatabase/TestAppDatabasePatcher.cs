using DFCommonLib.DataAccess;

namespace DFCommonLib.TestAppClient.DataAccess
{
    public class TestAppDatabasePatcher : StartupDatabasePatcher
    {
        private static string PATCHER = "TestApp";

        public TestAppDatabasePatcher(IDBPatcher dbPatcher) : base(dbPatcher)
        {
        }

        public override bool RunPatcher()
        {
            base.RunPatcher();

            // Test to create a table with patcher
            _dbPatcher.Patch(PATCHER,2, "CREATE TABLE `testme` ("
            + " `id` int(11) NOT NULL AUTO_INCREMENT, " 
            + " PRIMARY KEY (`id`)"
            + ")"
            );

            // Create OAuth2 tables
            _dbPatcher.Patch(PATCHER, 3, DFCommonLib.HttpApi.OAuth2.ServerOAuth2Repository.GetCreateTableString());

            return _dbPatcher.Successful();
        }
    }
}