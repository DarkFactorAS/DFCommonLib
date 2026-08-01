using DFCommonLib.Utils;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project DFCommonLib.ConfigEncryptor -- <value>");
    Console.Write("Value to encrypt: ");

    var inputFromPrompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(inputFromPrompt))
    {
        Console.Error.WriteLine("No value provided.");
        return;
    }

    Console.WriteLine(DFCrypt.Encrypt(inputFromPrompt));
    return;
}

var input = string.Join(" ", args);
Console.WriteLine(DFCrypt.Encrypt(input));
