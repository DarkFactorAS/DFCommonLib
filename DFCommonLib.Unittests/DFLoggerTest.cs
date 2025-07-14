using NUnit.Framework;
using Moq;
using DFCommonLib.Logger;
using System.Collections.Generic;

namespace DFCommonLib.Unittests
{
    [TestFixture]
    public class DFLoggerTest
    {
        private Mock<ILogOutputWriter> mockOutputWriter;
        private DFLogger<object> logger;

        [SetUp]
        public void Setup()
        {
            mockOutputWriter = new Mock<ILogOutputWriter>();
            DFLogger.AddOutput(DFLogLevel.INFO, mockOutputWriter.Object);
            logger = new DFLogger<object>();
        }

        [Test]
        public void TestStartup()
        {
            logger.Startup("TestApp", "1.0");
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.INFO, "Object", "Init application TestApp v:1.0"), Times.Once);
        }

        [Test]
        public void TestLogInfo()
        {
            logger.LogInfo("Info message");
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.INFO, "Object", "Info message"), Times.Once);
        }

        [Test]
        public void TestLogDebug()
        {
            logger.LogDebug("Debug message");
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.DEBUG, "Object", "Debug message"), Times.Once);
        }

        [Test]
        public void TestLogImportant()
        {
            logger.LogImportant("Important message");
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.IMPORTANT, "Object", "Important message"), Times.Once);
        }

        [Test]
        public void TestLogWarning()
        {
            logger.LogWarning("Warning message");
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.WARNING, "Object", "Warning message"), Times.Once);
        }

        [Test]
        public void TestLogError()
        {
            mockOutputWriter.Setup(x => x.LogMessage(DFLogLevel.ERROR, "Object", "Error message")).Returns(1);
            int errorCode = logger.LogError("Error message");
            Assert.AreEqual(1, errorCode);
            mockOutputWriter.Verify(x => x.LogMessage(DFLogLevel.ERROR, "Object", "Error message"), Times.Once);
        }
    }
}