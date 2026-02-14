using NUnit.Framework;
using Moq;
using DFCommonLib.DataAccess;
using System.Data;

namespace DFCommonLib.Unittests
{
    [TestFixture]
    public class MySQLDbDataReaderTest
    {
        private Mock<IDataReader> mockReader;

        [SetUp]
        public void Setup()
        {
            mockReader = new Mock<IDataReader>();
        }

        [Test]
        public void Constructor_ShouldCreateReader()
        {
            using (var reader = new MySQLDbDataReader(mockReader.Object, "SELECT 1"))
            {
                Assert.That(reader, Is.Not.Null);
            }
        }

        [Test]
        public void FetchSize_Get_ShouldReturnZero()
        {
            using (var reader = new MySQLDbDataReader(mockReader.Object, "SELECT 1"))
            {
                var fetchSize = reader.FetchSize;
                Assert.That(fetchSize, Is.EqualTo(0));
            }
        }

        [Test]
        public void FetchSize_Set_ShouldNotThrow()
        {
            using (var reader = new MySQLDbDataReader(mockReader.Object, "SELECT 1"))
            {
                Assert.DoesNotThrow(() => reader.FetchSize = 100);
            }
        }

        [Test]
        public void Dispose_ShouldDisposeUnderlyingReader()
        {
            var reader = new MySQLDbDataReader(mockReader.Object, "SELECT 1");
            reader.Dispose();
            
            mockReader.Verify(x => x.Dispose(), Times.Once);
        }

        [Test]
        public void Read_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.Read()).Returns(true);
            
            using (var reader = new MySQLDbDataReader(mockReader.Object, "SELECT 1"))
            {
                var result = reader.Read();
                Assert.That(result, Is.True);
            }
            
            mockReader.Verify(x => x.Read(), Times.Once);
        }
    }
}
