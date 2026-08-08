namespace DFCommonLib.Unittests;

using DFCommonLib.Config;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;

public class DFConfigTest
{
    TestAppSetting appSettings;

    [SetUp]
    public void Setup()
    {
        var mockEnvironment = new Mock<IHostEnvironment>();
        mockEnvironment
            .Setup(m => m.EnvironmentName)
            .Returns("Development");

        var helper = new ConfigurationHelper<TestAppSetting>(mockEnvironment.Object);
        Assert.That(helper.Settings, Is.Not.Null, "AppSettings could not be initialized.");
        Assert.That(helper.Settings, Is.InstanceOf<TestAppSetting>(), "AppSettings is not of type TestAppSetting.");
        appSettings = (TestAppSetting)helper.Settings;
    }

    [Test]
    public void TestReadConfigName()
    {
        Assert.That(appSettings.AppName, Is.EqualTo("TestApp"));
        Assert.That(appSettings.IsConfigEncrypted, Is.False);
        Assert.That(appSettings.EnableLogging, Is.True);
        Assert.That(appSettings.DatabaseConnection.Server, Is.EqualTo("DatabaseServer"));
        Assert.That(appSettings.DatabaseConnection.Database, Is.EqualTo("testdatabase"));
        Assert.That(appSettings.DatabaseConnection.Username, Is.EqualTo("dbuser"));
        Assert.That(appSettings.DatabaseConnection.Password, Is.EqualTo("dbpass"));

        // Extended
        Assert.That(appSettings.CommonLibServer?.Endpoint, Is.EqualTo("http://127.0.0.1:7000"));
        Assert.That(appSettings.CommonLibServer?.ClientId, Is.EqualTo("test_client_id"));
        Assert.That(appSettings.CommonLibServer?.ClientSecret, Is.EqualTo("test_client_secret"));
        Assert.That(appSettings.CommonLibServer?.Scope, Is.EqualTo("read write"));
    }

    [Test]
    public void TestReadEncryptedConfigValues()
    {
        var previousKey = Environment.GetEnvironmentVariable("TestApp__EncryptionKey");
        Environment.SetEnvironmentVariable("TestApp__EncryptionKey", "DarkFactor-DFCommonLib-2026-Default-Key");

        try
        {
            var mockEnvironment = new Mock<IHostEnvironment>();
            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Encrypted");

            var helper = new ConfigurationHelper<TestAppSetting>(mockEnvironment.Object);
            var encryptedSettings = (TestAppSetting)helper.Settings;

            Assert.That(encryptedSettings.IsConfigEncrypted, Is.True);
            Assert.That(encryptedSettings.AppName, Is.EqualTo("TestApp"));
            Assert.That(encryptedSettings.DatabaseConnection.Server, Is.EqualTo("DatabaseServer"));
            Assert.That(encryptedSettings.DatabaseConnection.Database, Is.EqualTo("testdatabase"));
            Assert.That(encryptedSettings.DatabaseConnection.Username, Is.EqualTo("dbuser"));
            Assert.That(encryptedSettings.DatabaseConnection.Password, Is.EqualTo("dbpass"));
            Assert.That(encryptedSettings.CommonLibServer?.Endpoint, Is.EqualTo("http://127.0.0.1:7000"));
            Assert.That(encryptedSettings.CommonLibServer?.ClientId, Is.EqualTo("test_client_id"));
            Assert.That(encryptedSettings.CommonLibServer?.ClientSecret, Is.EqualTo("test_client_secret"));
            Assert.That(encryptedSettings.CommonLibServer?.Scope, Is.EqualTo("read write"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("TestApp__EncryptionKey", previousKey);
        }
    }
}
