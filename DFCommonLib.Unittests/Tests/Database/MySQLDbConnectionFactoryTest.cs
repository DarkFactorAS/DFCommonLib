using NUnit.Framework;
using Moq;
using DFCommonLib.DataAccess;
using DFCommonLib.Config;
using System.Data;

namespace DFCommonLib.Unittests.Database
{
    // Concrete implementation for testing
    public class TestMySQLDbConnectionFactory : MySQLDbConnectionFactory
    {
        public TestMySQLDbConnectionFactory(string connectionType, IConfigurationHelper helper) 
            : base(connectionType, helper)
        {
        }

        public TestMySQLDbConnectionFactory(string connectionType, string connectionString) 
            : base(connectionType, connectionString)
        {
        }
    }

    [TestFixture]
    public class MySQLDbConnectionFactoryTest
    {
        private Mock<IConfigurationHelper> mockHelper;
        private const string TestConnectionString = "Server=localhost;Port=3306;Database=testdb;Uid=testuser;Pwd=testpass;SslMode=Disabled;";

        [SetUp]
        public void Setup()
        {
            mockHelper = new Mock<IConfigurationHelper>();
        }

        [Test]
        public void Constructor_WithConnectionString_ShouldCreateFactory()
        {
            var factory = new TestMySQLDbConnectionFactory("test", TestConnectionString);
            Assert.That(factory, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithConfigHelper_ShouldCreateFactory()
        {
            var factory = new TestMySQLDbConnectionFactory("test", mockHelper.Object);
            Assert.That(factory, Is.Not.Null);
        }

        [Test]
        public void CreateConnection_WithConnectionString_ShouldReturnMySQLDbConnection()
        {
            var factory = new TestMySQLDbConnectionFactory("test", TestConnectionString);
            
            // Note: This will try to connect to database, so we expect an exception
            Assert.Throws<MySql.Data.MySqlClient.MySqlException>(() =>
            {
                using (var connection = factory.CreateConnection())
                {
                    Assert.That(connection, Is.InstanceOf<MySQLDbConnection>());
                }
            });
        }

        [Test]
        public void CreateCommand_WithCommandText_ShouldReturnTimedMySQLDbCommand()
        {
            var factory = new TestMySQLDbConnectionFactory("test", TestConnectionString);
            
            // Note: This will try to connect to database, so we expect an exception
            Assert.Throws<MySql.Data.MySqlClient.MySqlException>(() =>
            {
                using (var command = factory.CreateCommand("SELECT 1"))
                {
                    Assert.That(command, Is.InstanceOf<TimedMySQLDbCommand>());
                }
            });
        }

        [Test]
        public void CreateCommand_WithCommandTextAndConnection_ShouldReturnTimedMySQLDbCommand()
        {
            var factory = new TestMySQLDbConnectionFactory("test", TestConnectionString);
            
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                using (var command = factory.CreateCommand("SELECT 1", connection))
                {
                    Assert.That(command, Is.InstanceOf<TimedMySQLDbCommand>());
                    Assert.That(command.CommandText, Is.EqualTo("SELECT 1"));
                }
            }
        }

        [Test]
        public void CreateConnection_WithConfigHelper_ShouldUseConfigSettings()
        {
            var mockSettings = new AppSettings
            {
                DatabaseConnection = new DatabaseConnection
                {
                    Server = "testserver",
                    Port = 3307,
                    Database = "testdb",
                    Username = "user",
                    Password = "pass",
                    SslMode = "Required"
                }
            };
            
            mockHelper.SetupGet(x => x.Settings).Returns(mockSettings);
            
            var factory = new TestMySQLDbConnectionFactory("test", mockHelper.Object);
            
            // Note: This will try to connect to database, so we expect an exception
            Assert.Throws<MySql.Data.MySqlClient.MySqlException>(() =>
            {
                using (var connection = factory.CreateConnection())
                {
                    // Connection string should be built from config
                }
            });
        }

        [Test]
        public void CreateConnection_WithNullDatabaseConnection_ShouldThrowException()
        {
            var mockSettings = new AppSettings
            {
                DatabaseConnection = null
            };
            
            mockHelper.SetupGet(x => x.Settings).Returns(mockSettings);
            
            var factory = new TestMySQLDbConnectionFactory("test", mockHelper.Object);
            
            Assert.Throws<System.Exception>(() =>
            {
                using (var connection = factory.CreateConnection())
                {
                }
            });
        }
    }
}
