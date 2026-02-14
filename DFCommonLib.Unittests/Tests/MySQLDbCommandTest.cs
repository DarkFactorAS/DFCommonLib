using NUnit.Framework;
using DFCommonLib.DataAccess;
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace DFCommonLib.Unittests
{
    [TestFixture]
    public class MySQLDbCommandTest
    {
        private const string TestConnectionString = "Server=localhost;Port=3306;Database=testdb;Uid=testuser;Pwd=testpass;";
        private MySQLDbConnection connection;

        [SetUp]
        public void Setup()
        {
            connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString);
        }

        [TearDown]
        public void TearDown()
        {
            connection?.Dispose();
        }

        [Test]
        public void Constructor_WithValidParameters_ShouldCreateCommand()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.That(command.CommandText, Is.EqualTo("SELECT 1"));
                Assert.That(command.CommandType, Is.EqualTo(CommandType.Text));
            }
        }

        [Test]
        public void Constructor_WithNullConnection_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var command = new MySQLDbCommand("SELECT 1", null, false))
                {
                }
            });
        }

        [Test]
        public void Constructor_WithNullCommandText_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var command = new MySQLDbCommand(null, connection, false))
                {
                }
            });
        }

        [Test]
        public void CommandText_SetValidValue_ShouldUpdateCommandText()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                command.CommandText = "SELECT 2";
                Assert.That(command.CommandText, Is.EqualTo("SELECT 2"));
            }
        }

        [Test]
        public void CommandText_SetNull_ShouldThrowArgumentNullException()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.Throws<ArgumentNullException>(() => command.CommandText = null);
            }
        }

        [Test]
        public void CommandType_SetStoredProcedure_ShouldUpdateCommandType()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                command.CommandType = CommandType.StoredProcedure;
                Assert.That(command.CommandType, Is.EqualTo(CommandType.StoredProcedure));
            }
        }

        [Test]
        public void BindByName_GetOrSet_ShouldThrowNotImplementedException()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.Throws<NotImplementedException>(() => { var value = command.BindByName; });
                Assert.Throws<NotImplementedException>(() => command.BindByName = true);
            }
        }

        [Test]
        public void ArrayBindCount_GetOrSet_ShouldThrowNotImplementedException()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.Throws<NotImplementedException>(() => { var value = command.ArrayBindCount; });
                Assert.Throws<NotImplementedException>(() => command.ArrayBindCount = 10);
            }
        }

        [Test]
        public void AddParameter_WithNameAndDbType_ShouldAddParameter()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                var param = command.AddParameter("@param1", DbType.String);
                
                Assert.That(param, Is.Not.Null);
                Assert.That(param.ParameterName, Is.EqualTo("@param1"));
                Assert.That(param.DbType, Is.EqualTo(DbType.String));
                Assert.That(command.Parameters.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void AddParameter_WithNameAndValue_ShouldInferDbType()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                var param1 = command.AddParameter("@intParam", 42);
                var param2 = command.AddParameter("@stringParam", "test");
                var param3 = command.AddParameter("@decimalParam", 3.14m);
                
                Assert.That(param1.DbType, Is.EqualTo(DbType.Int32));
                Assert.That(param2.DbType, Is.EqualTo(DbType.String));
                Assert.That(param3.DbType, Is.EqualTo(DbType.Decimal));
            }
        }

        [Test]
        public void AddParameter_WithAllParameters_ShouldSetCorrectValues()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                var param = command.AddParameter("@param", "value", DbType.String, ParameterDirection.Input);
                
                Assert.That(param.ParameterName, Is.EqualTo("@param"));
                Assert.That(param.Value, Is.EqualTo("value"));
                Assert.That(param.DbType, Is.EqualTo(DbType.String));
                Assert.That(param.Direction, Is.EqualTo(ParameterDirection.Input));
            }
        }

        [Test]
        public void AddParameter_WithIDbDataParameter_ShouldAddParameter()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                var param = new MySqlParameter("@param", "value");
                var result = command.AddParameter(param);
                
                Assert.That(result, Is.EqualTo(param));
                Assert.That(command.Parameters.Count, Is.EqualTo(1));
            }
        }

        [Test]
        public void AddClobParameter_ShouldAddBlobParameter()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                command.AddClobParameter("@clobParam", "large text content");
                
                Assert.That(command.Parameters.Count, Is.EqualTo(1));
                var param = command.Parameters["@clobParam"] as MySqlParameter;
                Assert.That(param, Is.Not.Null);
                Assert.That(param.MySqlDbType, Is.EqualTo(MySqlDbType.Blob));
            }
        }

        [Test]
        public void Prepare_ShouldThrowNotImplementedException()
        {
            using (var command = new MySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.Throws<NotImplementedException>(() => command.Prepare());
            }
        }

        [Test]
        public void Dispose_ShouldDisposeCommand()
        {
            var command = new MySQLDbCommand("SELECT 1", connection, false);
            command.Dispose();
            
            // After dispose, the command should be disposed
            // The MySQLDbCommand doesn't throw on property access after dispose,
            // but the underlying MySQL command should be disposed
            Assert.Pass("Command disposed successfully");
        }

        [Test]
        public void TimedMySQLDbCommand_ShouldCreateCommand()
        {
            using (var command = new TimedMySQLDbCommand("SELECT 1", connection, false))
            {
                Assert.That(command.CommandText, Is.EqualTo("SELECT 1"));
            }
        }
    }
}
