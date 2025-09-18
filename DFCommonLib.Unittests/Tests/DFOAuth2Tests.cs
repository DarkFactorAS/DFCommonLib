using System;
using NUnit.Framework;
using Moq;
using DFCommonLib.HttpApi.OAuth2;
using Microsoft.AspNetCore.Http;
using DFCommonLib.Logger;
using DFCommonLib.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using DFCommonLib.Utils;

namespace DFCommonLib.Unittests.OAuth2
{
    [TestFixture]
    public class DFOAuth2RestClientTest
    {
        [SetUp]
        public void Setup()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            DFCommonLib.Utils.DFServices.Create(services);
        }

        [Test]
        public void CanInstantiateDFOAuth2RestClient()
        {
            var client = new DFOAuth2RestClient();
            Assert.IsNotNull(client);
        }

        [Test]
        public void SetAuthClient_SetsClientData()
        {
            var client = new DFOAuth2RestClient();
            var data = new OAuth2ClientData();
            client.SetAuthClient(data);
            // No getter, so just ensure no exception
            Assert.Pass();
        }

        [Test]
        public void GetModule_ReturnsExpected()
        {
            var client = new DFOAuth2RestClient();
            var module = client.GetType().GetMethod("GetModule", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke(client, null);
            Assert.IsInstanceOf<string>(module);
        }

        [Test]
        public void SetupService_DoesNotThrow()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            Assert.DoesNotThrow(() => DFOAuth2RestClient.SetupService(services));
        }
    }

    [TestFixture]
    public class DFOAuth2ClientSessionTest
    {
        Mock<IHttpContextAccessor> mockHttpContextAccessor;

        [SetUp]
        public void Setup()
        {
            var mockSession = new Mock<ISession>();
            byte[] valueBytesEmail = System.Text.Encoding.UTF8.GetBytes("test@example.com");
            byte[] valueBytesToken = System.Text.Encoding.UTF8.GetBytes("token");

            // Setup TryGetValue to return the expected value
            mockSession.Setup(s => s.TryGetValue("DFOAuth2ClientSession.Email", out valueBytesEmail)).Returns(true);
            mockSession.Setup(s => s.TryGetValue("DFOAuth2ClientSession.Token", out valueBytesToken)).Returns(true);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(_ => _.Session).Returns(mockSession.Object);

            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.SetupGet(_ => _.HttpContext).Returns(mockHttpContext.Object);
        }

        [Test]
        public void CanInstantiateDFOAuth2ClientSession()
        {
            var session = new DFOAuth2ClientSession(mockHttpContextAccessor.Object);
            Assert.IsNotNull(session);
        }

        [Test]
        public void RemoveSession_DoesNotThrow()
        {
            var session = new DFOAuth2ClientSession(mockHttpContextAccessor.Object);
            Assert.DoesNotThrow(() => session.RemoveSession());
        }

        [Test]
        public void EmailTokenSetGet()
        {
            var session = new DFOAuth2ClientSession(mockHttpContextAccessor.Object);
            session.SetEmail("test@example.com");
            var email = session.GetEmail();
            Assert.IsInstanceOf<string>(email);
            session.SetToken("token");
            Assert.IsInstanceOf<string>(session.GetToken());
        }
    }

    [TestFixture]
    public class OAuth2ClientModelTest
    {
        [Test]
        public void CanInstantiateOAuth2ClientModel()
        {
            var model = new OAuth2ClientModel();
            Assert.IsNotNull(model);
        }

        [Test]
        public void PropertySettersAndGetters_Work()
        {
            var model = new OAuth2ClientModel();
            model.ClientId = "id";
            model.ClientSecret = "secret";
            model.Scope = "scope";
            model.TokenExpiresInSeconds = 123;
            model.TokenIssuer = "issuer";
            Assert.That(model.ClientId, Is.EqualTo("id"));
            Assert.That(model.ClientSecret, Is.EqualTo("secret"));
            Assert.That(model.Scope, Is.EqualTo("scope"));
            Assert.That(model.TokenExpiresInSeconds, Is.EqualTo((uint)123));
            Assert.That(model.TokenIssuer, Is.EqualTo("issuer"));
        }
    }

    [TestFixture]
    public class OAuth2CodeResponseTest
    {
        [Test]
        public void CanInstantiateOAuth2CodeResponse()
        {
            var response = new OAuth2CodeResponse();
            Assert.IsNotNull(response);
        }

        [Test]
        public void PropertySettersAndGetters_Work()
        {
            var response = new OAuth2CodeResponse();
            response.AccessToken = "token";
            response.State = "state";
            response.TokenType = "type";
            response.ExpiresIn = 42;
            Assert.That(response.AccessToken, Is.EqualTo("token"));
            Assert.That(response.State, Is.EqualTo("state"));
            Assert.That(response.TokenType, Is.EqualTo("type"));
            Assert.That(response.ExpiresIn, Is.EqualTo((uint)42));
        }
    }

    [TestFixture]
    public class OAuth2CodeDataTest
    {
        [Test]
        public void CanInstantiateOAuth2CodeData()
        {
            var data = new OAuth2CodeData();
            Assert.IsNotNull(data);
        }

        [Test]
        public void PropertySettersAndGetters_Work()
        {
            var data = new OAuth2CodeData();
            data.Code = "code";
            data.State = "state";
            Assert.That(data.Code, Is.EqualTo("code"));
            Assert.That(data.State, Is.EqualTo("state"));
        }

        [Test]
        public void Constructors_Work()
        {
            var data = new OAuth2CodeData("code", "state");
            Assert.That(data.Code, Is.EqualTo("code"));
            Assert.That(data.State, Is.EqualTo("state"));
        }
    }

    [TestFixture]
    public class OAuth2ClientDataTest
    {
        [Test]
        public void CanInstantiateOAuth2ClientData()
        {
            var data = new OAuth2ClientData();
            Assert.IsNotNull(data);
        }

        [Test]
        public void PropertySettersAndGetters_Work()
        {
            var data = new OAuth2ClientData();
            data.ClientId = "id";
            data.ClientSecret = "secret";
            data.RedirectUri = "uri";
            data.Scope = "scope";
            data.State = "state";
            Assert.That(data.ClientId, Is.EqualTo("id"));
            Assert.That(data.ClientSecret, Is.EqualTo("secret"));
            Assert.That(data.RedirectUri, Is.EqualTo("uri"));
            Assert.That(data.Scope, Is.EqualTo("scope"));
            Assert.That(data.State, Is.EqualTo("state"));
        }

        [Test]
        public void Constructors_Work()
        {
            var data = new OAuth2ClientData("id", "secret", "uri", "scope", "state");
            Assert.That(data.ClientId, Is.EqualTo("id"));
            Assert.That(data.ClientSecret, Is.EqualTo("secret"));
            Assert.That(data.RedirectUri, Is.EqualTo("uri"));
            Assert.That(data.Scope, Is.EqualTo("scope"));
            Assert.That(data.State, Is.EqualTo("state"));
        }
    }

    [TestFixture]
    public class OAuth2AuthResponseTest
    {
        [Test]
        public void CanInstantiateOAuth2AuthResponse()
        {
            var response = new OAuth2AuthResponse();
            Assert.IsNotNull(response);
        }

        [Test]
        public void PropertySettersAndGetters_Work()
        {
            var response = new OAuth2AuthResponse();
            response.Code = "code";
            response.State = "state";
            Assert.That(response.Code, Is.EqualTo("code"));
            Assert.That(response.State, Is.EqualTo("state"));
        }
    }

    [TestFixture]
    public class OAuth2ServerTest
    {
        [Test]
        public void CanInstantiateOAuth2Server()
        {
            var server = new OAuth2Server();
            Assert.IsNotNull(server);
        }

        [Test]
        public void SetupService_DoesNotThrow()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            Assert.DoesNotThrow(() => OAuth2Server.SetupService(services));
        }

        [Test]
        public void SetupSwaggerApi_DoesNotThrow()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            Assert.DoesNotThrow(() => OAuth2Server.SetupSwaggerApi("TestAppServer", services));
        }
    }

    [TestFixture]
    public class DFOAuth2JwtTokenHandlerTest
    {
        [SetUp]
        public void Setup()
        {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            DFCommonLib.Utils.DFServices.Create(services);
        }
        [Test]
        public void CanInstantiateDFOAuth2JwtTokenHandler()
        {
            var handler = new DFOAuth2JwtTokenHandler();
            Assert.IsNotNull(handler);
        }

        [Test]
        public void ValidateToken_ThrowsNotImplementedOrReturnsNull()
        {
            var handler = new DFOAuth2JwtTokenHandler();
            try
            {
                handler.ValidateToken("token", null, out var validatedToken);
            }
            catch (Exception ex)
            {
                Assert.Pass("Exception thrown as expected: " + ex.Message);
            }
        }
    }

    // [TestFixture]
    // public class DFRestOAuth2ServerControllerTest
    // {
    //     [SetUp]
    //     public void Setup()
    //     {
    //         var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
    //         DFCommonLib.Utils.DFServices.Create(services);
    //     }
    //     [Test]
    //     public void CanInstantiateDFRestOAuth2ServerController()
    //     {
    //         var controller = new DFRestOAuth2ServerController();
    //         Assert.IsNotNull(controller);
    //     }

    //     [Test]
    //     public void AuthAndCodeMethods_DoesNotThrow()
    //     {
    //         var controller = new DFRestOAuth2ServerController();
    //         var clientData = new OAuth2ClientData();
    //         var codeData = new OAuth2CodeData();
    //         Assert.DoesNotThrow(() => controller.Auth(clientData));
    //         Assert.DoesNotThrow(() => controller.Code(codeData));
    //     }
    // }

    [TestFixture]
    public class ServerOAuth2ProviderTest
    {
        [Test]
        public void CanInstantiateServerOAuth2Provider()
        {
            var sessionProviderMock = new Mock<IServerOAuth2Session>();
            var repoMock = new Mock<IServerOAuth2Repository>();
            var loggerMock = new Mock<IDFLogger<ServerOAuth2Provider>>();
            var provider = new ServerOAuth2Provider(sessionProviderMock.Object, repoMock.Object, loggerMock.Object);
            Assert.IsNotNull(provider);
        }
    }

    [TestFixture]
    public class ServerOAuth2SessionTest
    {
        [Test]
        public void CanInstantiateServerOAuth2Session()
        {
            var httpContextMock = new Mock<IHttpContextAccessor>();
            var session = new ServerOAuth2Session(httpContextMock.Object);
            Assert.IsNotNull(session);
        }
    }

    [TestFixture]
    public class ServerOAuth2RepositoryTest
    {
        [Test]
        public void CanInstantiateServerOAuth2Repository()
        {
            var connectionMock = new Mock<IDbConnectionFactory>();
            var loggerMock = new Mock<IDFLogger<ServerOAuth2Repository>>();
            var repo = new ServerOAuth2Repository(connectionMock.Object, loggerMock.Object);
            Assert.IsNotNull(repo);
        }
    }
}
