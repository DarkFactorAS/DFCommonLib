using NUnit.Framework;
using Moq;
using DFCommonLib.DataAccess;

namespace DFCommonLib.Unittests.Database
{
    [TestFixture]
    public class DbConnectionPoolTest
    {
        private DbConnectionPool pool;
        private Mock<IDbConnectionFactory> mockFactory1;
        private Mock<IDbConnectionFactory> mockFactory2;

        [SetUp]
        public void Setup()
        {
            pool = new DbConnectionPool();
            mockFactory1 = new Mock<IDbConnectionFactory>();
            mockFactory2 = new Mock<IDbConnectionFactory>();
        }

        [Test]
        public void AddConnection_ShouldAddNewConnection()
        {
            pool.AddConnection("customer1", mockFactory1.Object);
            
            var result = pool.GetConnection("customer1");
            
            Assert.That(result, Is.EqualTo(mockFactory1.Object));
        }

        [Test]
        public void AddConnection_ShouldNotAddDuplicateCustomer()
        {
            pool.AddConnection("customer1", mockFactory1.Object);
            pool.AddConnection("customer1", mockFactory2.Object);
            
            var result = pool.GetConnection("customer1");
            
            Assert.That(result, Is.EqualTo(mockFactory1.Object));
        }

        [Test]
        public void GetConnection_ShouldReturnNullForNonExistentCustomer()
        {
            var result = pool.GetConnection("nonexistent");
            
            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddConnection_ShouldSupportMultipleCustomers()
        {
            pool.AddConnection("customer1", mockFactory1.Object);
            pool.AddConnection("customer2", mockFactory2.Object);
            
            var result1 = pool.GetConnection("customer1");
            var result2 = pool.GetConnection("customer2");
            
            Assert.That(result1, Is.EqualTo(mockFactory1.Object));
            Assert.That(result2, Is.EqualTo(mockFactory2.Object));
        }
    }
}
