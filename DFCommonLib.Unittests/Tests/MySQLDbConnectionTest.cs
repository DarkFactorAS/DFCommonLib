using NUnit.Framework;
using DFCommonLib.DataAccess;
using System;
using System.Data;

namespace DFCommonLib.Unittests
{
    [TestFixture]
    public class MySQLDbConnectionTest
    {
        private const string TestConnectionString = "Server=localhost;Port=3306;Database=testdb;Uid=testuser;Pwd=testpass;";

        [Test]
        public void Constructor_WithConnectionString_ShouldSetConnectionStateOpen()
        {
            // Note: This will try to actually connect, so we expect an exception
            // since the test database doesn't exist. We're testing the constructor logic.
            Assert.Throws<MySql.Data.MySqlClient.MySqlException>(() =>
            {
                using (var connection = new MySQLDbConnection(TestConnectionString))
                {
                    // Connection attempts to open
                }
            });
        }

        [Test]
        public void Constructor_WithClosedState_ShouldNotThrow()
        {
            // With ConnectionState.Closed, it should not try to connect
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                var dbConnection = connection as IDbConnection;
                Assert.That(dbConnection.State, Is.EqualTo(ConnectionState.Closed));
            }
        }

        [Test]
        public void Constructor_WithInvalidState_ShouldThrowNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() =>
            {
                using (var connection = new MySQLDbConnection(ConnectionState.Connecting, TestConnectionString))
                {
                }
            });
        }

        [Test]
        public void IDbConnection_ConnectionString_ShouldReturnCorrectValue()
        {
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                var dbConnection = connection as IDbConnection;
                // MySQL normalizes the connection string (lowercase keys)
                Assert.That(dbConnection.ConnectionString.ToLower(), Does.Contain("server=localhost"));
                Assert.That(dbConnection.ConnectionString.ToLower(), Does.Contain("database=testdb"));
            }
        }

        [Test]
        public void IDbConnection_ChangeDatabase_ShouldThrowException()
        {
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                var dbConnection = connection as IDbConnection;
                Assert.Throws<Exception>(() => dbConnection.ChangeDatabase("newdb"));
            }
        }

        [Test]
        public void IDbConnection_Database_ShouldThrowException()
        {
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                var dbConnection = connection as IDbConnection;
                Assert.Throws<Exception>(() => { var db = dbConnection.Database; });
            }
        }

        [Test]
        public void WrappedConnection_ShouldReturnUnderlyingConnection()
        {
            using (var connection = new MySQLDbConnection(ConnectionState.Closed, TestConnectionString))
            {
                Assert.That(connection.WrappedConnection, Is.Not.Null);
                Assert.That(connection.WrappedConnection, Is.InstanceOf<MySql.Data.MySqlClient.MySqlConnection>());
            }
        }
    }
}
