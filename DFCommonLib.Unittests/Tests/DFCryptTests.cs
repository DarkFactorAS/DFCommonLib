using DFCommonLib.Utils;

namespace DFCommonLib.Unittests;

public class DFCryptTests
{
    [Test]
    public void EncryptAndDecryptRoundTrip()
    {
        Environment.SetEnvironmentVariable("DFCommonLib_EncryptionKey", "test-key-for-unit-tests-do-not-use");
        const string plaintext = "super-secret-value";

        var encrypted = DFCrypt.Encrypt(plaintext);
        var decrypted = DFCrypt.Decrypt(encrypted);

        Assert.That(encrypted, Is.Not.Empty);
        Assert.That(decrypted, Is.EqualTo(plaintext));
    }

    [Test]
    public void Base64HelpersRoundTrip()
    {
        const string plaintext = "plain-base64-value";

        var encoded = DFCrypt.EncryptBase64(plaintext);
        var decoded = DFCrypt.DecryptBase64(encoded);

        Assert.That(encoded, Is.EqualTo("cGxhaW4tYmFzZTY0LXZhbHVl"));
        Assert.That(decoded, Is.EqualTo(plaintext));
    }
}
