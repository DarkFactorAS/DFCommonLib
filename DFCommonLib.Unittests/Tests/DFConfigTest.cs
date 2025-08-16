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
        Assert.That(appSettings.SecretKey, Is.EqualTo("TestSecret"));
        Assert.That(appSettings.EnableLogging, Is.True);
        Assert.That(appSettings.DatabaseConnection.Server, Is.EqualTo("DatabaseServer"));
        Assert.That(appSettings.DatabaseConnection.Database, Is.EqualTo("testdatabase"));
        Assert.That(appSettings.DatabaseConnection.Username, Is.EqualTo("dbuser"));
        Assert.That(appSettings.DatabaseConnection.Password, Is.EqualTo("dbpass"));

        // Extended
        Assert.That(appSettings.AccountServer?.Server, Is.EqualTo("127.0.0.1"));
        Assert.That(appSettings.AccountServer?.Port, Is.EqualTo(6606));
        Assert.That(appSettings.AccountServer?.ApiKey, Is.EqualTo("ApiKey"));
    }
}
