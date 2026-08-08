using DFCommonLib.Utils;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run --project DFCommonLib.ConfigEncryptor -- <encryptionKey> <value>");
    return 1;
}

var encryptionKey = args[0];
var input = string.Join(" ", args.Skip(1));

if (string.IsNullOrWhiteSpace(encryptionKey) || string.IsNullOrWhiteSpace(input))
{
    Console.Error.WriteLine("Both encryption key and value are required.");
    return 1;
}

try
{
    Console.WriteLine(DFCrypt.Encrypt(input, encryptionKey));
}
catch (ArgumentException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
return 0;
