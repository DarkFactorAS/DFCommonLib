using System.Diagnostics;
using System.Text.Json.Nodes;
using DFCommonLib.Utils;

namespace DFCommonLib.Unittests;

public class ConfigDecryptorTests
{
    private const string EncryptionKey = "test-key-for-unit-tests-do-not-use";

    [Test]
    public void Decryptor_DecryptsSingleValueArgument()
    {
        const string plaintext = "single-value-secret";
        var encrypted = DFCrypt.Encrypt(plaintext, EncryptionKey);

        var result = RunConfigDecryptor(EncryptionKey, encrypted);

        Assert.That(result.ExitCode, Is.EqualTo(0), $"stderr: {result.StandardError}");
        Assert.That(result.StandardOutput, Does.Contain(plaintext));
    }

    [Test]
    public void Decryptor_FileModeDecryptsJsonAndWritesOutFile()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), "dfcommonlib-decryptor-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDirectory);

        try
        {
            var inputFilePath = Path.Combine(tempDirectory, "appsettings.encrypted.json");
            var encryptedJson = new JsonObject
            {
                ["AppName"] = "TestApp",
                ["AppVersion"] = "1.0",
                ["EncryptionKey"] = "leave-this-unchanged",
                ["IsConfigEncrypted"] = true,
                ["DatabaseConnection"] = new JsonObject
                {
                    ["Server"] = DFCrypt.Encrypt("DatabaseServer", EncryptionKey),
                    ["Password"] = DFCrypt.Encrypt("dbpass", EncryptionKey)
                },
                ["ArrayValues"] = new JsonArray
                {
                    DFCrypt.Encrypt("item-1", EncryptionKey),
                    new JsonObject
                    {
                        ["Nested"] = DFCrypt.Encrypt("item-2", EncryptionKey)
                    }
                }
            };

            File.WriteAllText(inputFilePath, encryptedJson.ToJsonString());

            var result = RunConfigDecryptor(EncryptionKey, "--file", inputFilePath, "--out");

            Assert.That(result.ExitCode, Is.EqualTo(0), $"stderr: {result.StandardError}");

            var outputFilePath = Path.Combine(tempDirectory, "decrypted-appsettings.encrypted.json");
            Assert.That(File.Exists(outputFilePath), Is.True, "Expected --out file was not created.");

            var outputRoot = JsonNode.Parse(File.ReadAllText(outputFilePath)) as JsonObject;
            Assert.That(outputRoot, Is.Not.Null);

            Assert.That(outputRoot!["AppName"]?.GetValue<string>(), Is.EqualTo("TestApp"));
            Assert.That(outputRoot["AppVersion"]?.GetValue<string>(), Is.EqualTo("1.0"));
            Assert.That(outputRoot["EncryptionKey"]?.GetValue<string>(), Is.EqualTo("leave-this-unchanged"));
            Assert.That(outputRoot["IsConfigEncrypted"]?.GetValue<bool>(), Is.False);

            var db = outputRoot["DatabaseConnection"] as JsonObject;
            Assert.That(db, Is.Not.Null);
            Assert.That(db!["Server"]?.GetValue<string>(), Is.EqualTo("DatabaseServer"));
            Assert.That(db["Password"]?.GetValue<string>(), Is.EqualTo("dbpass"));

            var arrayValues = outputRoot["ArrayValues"] as JsonArray;
            Assert.That(arrayValues, Is.Not.Null);
            Assert.That(arrayValues![0]?.GetValue<string>(), Is.EqualTo("item-1"));
            Assert.That((arrayValues[1] as JsonObject)?["Nested"]?.GetValue<string>(), Is.EqualTo("item-2"));
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }

    private static ProcessResult RunConfigDecryptor(params string[] decryptorArguments)
    {
        var solutionRoot = GetSolutionRoot();
        var projectPath = Path.Combine(solutionRoot, "DFCommonLib.ConfigDecryptor", "DFCommonLib.ConfigDecryptor.csproj");

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = solutionRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(projectPath);
        startInfo.ArgumentList.Add("--");

        foreach (var arg in decryptorArguments)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = Process.Start(startInfo);
        Assert.That(process, Is.Not.Null, "Failed to start dotnet process.");

        if (!process!.WaitForExit(120000))
        {
            process.Kill(entireProcessTree: true);
            Assert.Fail("ConfigDecryptor process timed out.");
        }

        return new ProcessResult(process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var solutionPath = Path.Combine(directory.FullName, "DFCommonLib.sln");
            if (File.Exists(solutionPath))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        Assert.Fail("Could not locate solution root containing DFCommonLib.sln.");
        return string.Empty;
    }

    private readonly record struct ProcessResult(int ExitCode, string StandardOutput, string StandardError);
}
