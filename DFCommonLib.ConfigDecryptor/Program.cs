using DFCommonLib.Utils;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;

if (args.Length < 2)
{
    PrintUsage();
    return 1;
}

var encryptionKey = args[0];

if (string.IsNullOrWhiteSpace(encryptionKey))
{
    Console.Error.WriteLine("Encryption key is required.");
    return 1;
}

if (args[1] is "--file" or "-f")
{
    if (args.Length < 3)
    {
        Console.Error.WriteLine("File mode requires a file path argument.");
        PrintUsage();
        return 1;
    }

    var filePath = args[2];
    var trailingArgs = args.Skip(3).ToArray();
    var writeOutputFile = false;
    foreach (var arg in trailingArgs)
    {
        if (arg is "--out" or "-o")
        {
            writeOutputFile = true;
            continue;
        }

        Console.Error.WriteLine($"Unknown argument for file mode: {arg}");
        PrintUsage();
        return 1;
    }

    if (!File.Exists(filePath))
    {
        Console.Error.WriteLine($"Config file not found: {filePath}");
        return 1;
    }

    try
    {
        var fileContent = File.ReadAllText(filePath);
        var root = JsonNode.Parse(fileContent) as JsonObject;
        if (root == null)
        {
            Console.Error.WriteLine("Config file must contain a JSON object at the root.");
            return 1;
        }

        DecryptJsonObject(root, encryptionKey);
        root["IsConfigEncrypted"] = false;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var decryptedContent = root.ToJsonString(options);
        if (writeOutputFile)
        {
            var decryptedFilePath = BuildDecryptedFilePath(filePath);
            File.WriteAllText(decryptedFilePath, decryptedContent + Environment.NewLine);
            Console.WriteLine($"Decrypted configuration values written to '{decryptedFilePath}'.");
        }

        Console.WriteLine(decryptedContent);
        return 0;
    }
    catch (JsonException ex)
    {
        Console.Error.WriteLine($"Invalid JSON in config file: {ex.Message}");
        return 1;
    }
    catch (FormatException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
    catch (CryptographicException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
    catch (ArgumentException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
}

var input = string.Join(" ", args.Skip(1));
if (string.IsNullOrWhiteSpace(input))
{
    Console.Error.WriteLine("Value to decrypt is required.");
    PrintUsage();
    return 1;
}

try
{
    Console.WriteLine(DFCrypt.Decrypt(input, encryptionKey));
    return 0;
}
catch (FormatException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (CryptographicException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}

static void PrintUsage()
{
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project DFCommonLib.ConfigDecryptor -- <encryptionKey> <value>");
    Console.WriteLine("  dotnet run --project DFCommonLib.ConfigDecryptor -- <encryptionKey> --file <configFilePath> [--out]");
}

static string BuildDecryptedFilePath(string inputFilePath)
{
    var directory = Path.GetDirectoryName(inputFilePath) ?? string.Empty;
    var fileName = Path.GetFileName(inputFilePath);
    return Path.Combine(directory, $"decrypted-{fileName}");
}

static void DecryptJsonObject(JsonObject jsonObject, string encryptionKey)
{
    foreach (var property in jsonObject.ToList())
    {
        if (property.Value is null)
        {
            continue;
        }

        if (property.Value is JsonObject childObject)
        {
            DecryptJsonObject(childObject, encryptionKey);
            continue;
        }

        if (property.Value is JsonArray childArray)
        {
            DecryptJsonArray(childArray, encryptionKey);
            continue;
        }

        if (property.Value is JsonValue jsonValue &&
            !property.Key.Equals("AppName", StringComparison.OrdinalIgnoreCase) &&
            !property.Key.Equals("AppVersion", StringComparison.OrdinalIgnoreCase) &&
            !property.Key.Equals("EncryptionKey", StringComparison.OrdinalIgnoreCase) &&
            jsonValue.TryGetValue<string>(out var stringValue) &&
            !string.IsNullOrWhiteSpace(stringValue))
        {
            jsonObject[property.Key] = DFCrypt.Decrypt(stringValue, encryptionKey);
        }
    }
}

static void DecryptJsonArray(JsonArray jsonArray, string encryptionKey)
{
    for (var index = 0; index < jsonArray.Count; index++)
    {
        var item = jsonArray[index];
        if (item is null)
        {
            continue;
        }

        if (item is JsonObject childObject)
        {
            DecryptJsonObject(childObject, encryptionKey);
            continue;
        }

        if (item is JsonArray childArray)
        {
            DecryptJsonArray(childArray, encryptionKey);
            continue;
        }

        if (item is JsonValue jsonValue &&
            jsonValue.TryGetValue<string>(out var stringValue) &&
            !string.IsNullOrWhiteSpace(stringValue))
        {
            jsonArray[index] = DFCrypt.Decrypt(stringValue, encryptionKey);
        }
    }
}
