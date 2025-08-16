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
        public async Task TestHandleRequestMock()
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
    }
}
