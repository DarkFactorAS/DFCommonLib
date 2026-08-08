using System.Security.Cryptography;
using DFCommonLib.Utils;

namespace DFCommonLib.Unittests;

public class DFCryptTests
{
    private const string TestEncryptionKey = "test-key-for-unit-tests-do-not-use";

    [Test]
    public void EncryptAndDecryptRoundTrip()
    {
        const string plaintext = "super-secret-value";

        var encrypted = DFCrypt.Encrypt(plaintext, TestEncryptionKey);
        var decrypted = DFCrypt.Decrypt(encrypted, TestEncryptionKey);

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

    [Test]
    public void TamperedCiphertextThrowsCryptographicException()
    {
        const string plaintext = "tamper-test-value";

        var encrypted = DFCrypt.Encrypt(plaintext, TestEncryptionKey);
        var encryptedBytes = Convert.FromBase64String(encrypted);

        // Flip the last byte of the ciphertext to simulate tampering
        encryptedBytes[encryptedBytes.Length - 1] ^= 0xFF;
        var tampered = Convert.ToBase64String(encryptedBytes);

        Assert.That(() => DFCrypt.Decrypt(tampered, TestEncryptionKey), Throws.InstanceOf<CryptographicException>());
    }

    [Test]
    public void TooShortPayloadThrowsFormatException()
    {
        // 28 bytes is exactly NonceSizeBytes(12) + TagSizeBytes(16), which is too short (needs at least 1 byte of ciphertext)
        var tooShort = Convert.ToBase64String(new byte[28]);

        Assert.Throws<FormatException>(() => DFCrypt.Decrypt(tooShort, TestEncryptionKey));
    }
}
