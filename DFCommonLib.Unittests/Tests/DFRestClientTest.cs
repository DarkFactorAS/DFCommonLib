using System;
using NUnit.Framework;
using DFCommonLib.HttpApi;
using Moq;
using DFCommonLib.Logger;
using Moq.Protected;

namespace DFCommonLib.Unittests.Tests
{
    [TestFixture]
    public class DFRestClientTest
    {
        private Mock<IDFLogger<DFRestClient>> _mockLogger;
        private DFRestClient _mockClient;

        private class TestableDFRestClient : DFRestClient
        {
            private readonly WebAPIData _mockResponse;

            public TestableDFRestClient(IDFLogger<DFRestClient> logger, WebAPIData mockResponse) : base(logger)
            {
                _mockResponse = mockResponse;
            }

            protected override async Task<WebAPIData> HandleRequest(int methodId, HttpRequestMessage webRequest)
            {
                return await Task.FromResult(_mockResponse);
            }
        }

        private class TestDataObject : WebAPIData
        {
            public int extraStuff;

            public TestDataObject()
            {
                extraStuff = 0;
            }
        }

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<IDFLogger<DFRestClient>>();
            _mockClient = new DFRestClient(_mockLogger.Object);
        }

        [Test]
        public void TestDFRestClientInitialization()
        {
            // Assert
            Assert.That(_mockClient, Is.Not.Null);
        }

        [Test]
        public void TestSetAndGetEndpoint()
        {
            // Arrange
            var endpoint = "http://example.com";

            // Act
            _mockClient.SetEndpoint(endpoint);
            var result = _mockClient.GetEndpoint();

            // Assert
            Assert.That(result, Is.EqualTo(endpoint));
        }

        [Test]
        public async Task TestPingError()
        {
            // Arrange
            string message = "Ping Failed";
            var mockResponse = new WebAPIData(0, message);
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.Ping();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.Not.EqualTo(0));
            Assert.That(result.message, Is.Not.EqualTo(message));
        }

        [Test]
        public async Task TestPingSuccess()
        {
            // Arrange
            string message = "PONG";
            var mockResponse = new WebAPIData(0, message);
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.Ping();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo(message));
        }

        [Test]
        public async Task TestGetJsonData()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "Success");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.GetJsonData(1, "testUrl");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
        }


        [Test]
        public async Task TestGetJsonDataAs()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "{ \"errorCode\": 0, \"message\": \"Success\", \"extraStuff\": 42 }");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.GetJsonDataAs<TestDataObject>(1, "testUrl");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
            Assert.That(result.extraStuff, Is.EqualTo(42));
        }

        [Test]
        public async Task TestPutJsonData()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "Success");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PutJsonData(1, "testUrl", "IGNORE-TEXT");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
        }

        [Test]
        public async Task TestPutData()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "Success");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PutData(1, "testUrl", new TestDataObject());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
        }

        [Test]
        public async Task TestPutJsonDataAs()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "{ \"errorCode\": 0, \"message\": \"Success\", \"extraStuff\": 42 }");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PutJsonDataAs<TestDataObject>(1, "testUrl", "IGNORE");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
            Assert.That(result.extraStuff, Is.EqualTo(42));
        }

        [Test]
        public async Task TestPutDataAs()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "{ \"errorCode\": 0, \"message\": \"Success\", \"extraStuff\": 42 }");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PutDataAs<TestDataObject>(1, "testUrl", new TestDataObject());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
            Assert.That(result.extraStuff, Is.EqualTo(42));
        }


        //
        // Post functions
        //
        [Test]
        public async Task TestPostJsonData()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "Success");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PostJsonData(1, "testUrl", "IGNORE-TEXT");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
        }

        [Test]
        public async Task TestPostData()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "Success");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PostData(1, "testUrl", new TestDataObject());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
        }

        [Test]
        public async Task TestPostJsonDataAs()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "{ \"errorCode\": 0, \"message\": \"Success\", \"extraStuff\": 42 }");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PostJsonDataAs<TestDataObject>(1, "testUrl", "IGNORE");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
            Assert.That(result.extraStuff, Is.EqualTo(42));
        }            

        [Test]
        public async Task TestPostDataAs()
        {
            // Arrange
            var mockResponse = new WebAPIData(0, "{ \"errorCode\": 0, \"message\": \"Success\", \"extraStuff\": 42 }");
            var testClient = new TestableDFRestClient(_mockLogger.Object, mockResponse);

            // Act
            var result = await testClient.PostDataAs<TestDataObject>(1, "testUrl", new TestDataObject());

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.errorCode, Is.EqualTo(0));
            Assert.That(result.message, Is.EqualTo("Success"));
            Assert.That(result.extraStuff, Is.EqualTo(42));
        }            
    }
}
