using DFCommonLib.Utils;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project DFCommonLib.ConfigEncryptor -- <value>");
    Console.Write("Value to encrypt: ");

    var inputFromPrompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(inputFromPrompt))
    {
        Console.Error.WriteLine("No value provided.");
        return 1;
    }

    try
    {
        Console.WriteLine(DFCrypt.Encrypt(inputFromPrompt));
    }
    catch (InvalidOperationException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        return 1;
    }
    return 0;
}

var input = string.Join(" ", args);
try
{
    Console.WriteLine(DFCrypt.Encrypt(input));
}
catch (InvalidOperationException ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
return 0;
