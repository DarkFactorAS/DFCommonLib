using NUnit.Framework;
using Moq;
using DFCommonLib.DataAccess;
using System;
using System.Data;
using System.Collections;

namespace DFCommonLib.Unittests.Database
{
    [TestFixture]
    public class BaseDbDataReaderTest
    {
        private Mock<IDataReader> mockReader;
        private BaseDbDataReader reader;

        [SetUp]
        public void Setup()
        {
            // Create a mock that implements both IDataReader and IEnumerable
            mockReader = new Mock<IDataReader>();
            mockReader.As<IEnumerable>();
            reader = new BaseDbDataReader(mockReader.Object);
        }

        [Test]
        public void Constructor_WithNullReader_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new BaseDbDataReader(null));
        }

        [Test]
        public void Read_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.Read()).Returns(true);
            
            var result = reader.Read();
            
            Assert.That(result, Is.True);
            mockReader.Verify(x => x.Read(), Times.Once);
        }

        [Test]
        public void GetString_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.GetString(0)).Returns("test");
            
            var result = reader.GetString(0);
            
            Assert.That(result, Is.EqualTo("test"));
            mockReader.Verify(x => x.GetString(0), Times.Once);
        }

        [Test]
        public void IndexerByName_ShouldReturnValue()
        {
            mockReader.Setup(x => x["column1"]).Returns("value1");
            
            var result = reader["column1"];
            
            Assert.That(result, Is.EqualTo("value1"));
        }

        [Test]
        public void IndexerByIndex_ShouldReturnValue()
        {
            mockReader.Setup(x => x[0]).Returns("value1");
            
            var result = reader[0];
            
            Assert.That(result, Is.EqualTo("value1"));
        }

        [Test]
        public void GetValue_ShouldReturnTypedValue()
        {
            mockReader.Setup(x => x["column1"]).Returns(42);
            
            var result = reader.GetValue<int>("column1");
            
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void IDataReader_Close_ShouldCallUnderlyingReader()
        {
            var idr = reader as IDataReader;
            idr.Close();
            
            mockReader.Verify(x => x.Close(), Times.Once);
        }

        [Test]
        public void IDataReader_Depth_ShouldReturnUnderlyingValue()
        {
            mockReader.Setup(x => x.Depth).Returns(1);
            
            var idr = reader as IDataReader;
            var result = idr.Depth;
            
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void IDataReader_IsClosed_ShouldReturnUnderlyingValue()
        {
            mockReader.Setup(x => x.IsClosed).Returns(true);
            
            var idr = reader as IDataReader;
            var result = idr.IsClosed;
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void IDataReader_RecordsAffected_ShouldReturnUnderlyingValue()
        {
            mockReader.Setup(x => x.RecordsAffected).Returns(10);
            
            var idr = reader as IDataReader;
            var result = idr.RecordsAffected;
            
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void IDataRecord_FieldCount_ShouldReturnUnderlyingValue()
        {
            mockReader.Setup(x => x.FieldCount).Returns(5);
            
            var idr = reader as IDataRecord;
            var result = idr.FieldCount;
            
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void IDataRecord_GetBoolean_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.GetBoolean(0)).Returns(true);
            
            var idr = reader as IDataRecord;
            var result = idr.GetBoolean(0);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void IDataRecord_GetInt32_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.GetInt32(0)).Returns(42);
            
            var idr = reader as IDataRecord;
            var result = idr.GetInt32(0);
            
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void IDataRecord_GetDateTime_ShouldCallUnderlyingReader()
        {
            var testDate = new DateTime(2024, 1, 1);
            mockReader.Setup(x => x.GetDateTime(0)).Returns(testDate);
            
            var idr = reader as IDataRecord;
            var result = idr.GetDateTime(0);
            
            Assert.That(result, Is.EqualTo(testDate));
        }

        [Test]
        public void IDataRecord_GetDecimal_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.GetDecimal(0)).Returns(3.14m);
            
            var idr = reader as IDataRecord;
            var result = idr.GetDecimal(0);
            
            Assert.That(result, Is.EqualTo(3.14m));
        }

        [Test]
        public void IDataRecord_IsDBNull_ShouldCallUnderlyingReader()
        {
            mockReader.Setup(x => x.IsDBNull(0)).Returns(true);
            
            var idr = reader as IDataRecord;
            var result = idr.IsDBNull(0);
            
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetEnumerator_ShouldReturnEnumerator()
        {
            var mockEnumerator = new Mock<IEnumerator>();
            mockReader.As<IEnumerable>().Setup(x => x.GetEnumerator()).Returns(mockEnumerator.Object);
            
            var enumerator = reader.GetEnumerator();
            
            Assert.That(enumerator, Is.Not.Null);
        }

        [Test]
        public void Dispose_ShouldDisposeUnderlyingReader()
        {
            reader.Dispose();
            
            mockReader.Verify(x => x.Dispose(), Times.Once);
        }
    }
}
