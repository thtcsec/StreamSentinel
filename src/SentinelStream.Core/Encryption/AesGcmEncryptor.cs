using System.Security.Cryptography;
using System.Text;

namespace SentinelStream.Core.Encryption;

/// <summary>
/// AES-256-GCM Encryption Engine for E2EE stream protection.
/// Uses authenticated encryption — provides both confidentiality and integrity.
/// </summary>
public sealed class AesGcmEncryptor : IDisposable
{
    private readonly byte[] _key;
    private bool _disposed;

    /// <summary>
    /// Initializes the encryptor with a 256-bit (32-byte) key.
    /// </summary>
    /// <param name="hexKey">64-character hex string representing 32 bytes.</param>
    public AesGcmEncryptor(string hexKey)
    {
        if (string.IsNullOrWhiteSpace(hexKey))
            throw new ArgumentException("Encryption key cannot be null or empty.", nameof(hexKey));

        _key = Convert.FromHexString(hexKey);

        if (_key.Length != 32)
            throw new ArgumentException("AES-256 requires a 32-byte (64-hex-character) key.", nameof(hexKey));
    }

    /// <summary>
    /// Encrypts plaintext data using AES-256-GCM.
    /// Returns: [12-byte nonce][16-byte tag][ciphertext]
    /// </summary>
    public byte[] Encrypt(byte[] plaintext)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var nonce = new byte[12]; // GCM standard nonce size
        RandomNumberGenerator.Fill(nonce);

        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[16]; // GCM authentication tag

        using var aes = new AesGcm(_key, 16);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        // Pack: [nonce (12)] + [tag (16)] + [ciphertext (N)]
        var result = new byte[nonce.Length + tag.Length + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(ciphertext, 0, result, nonce.Length + tag.Length, ciphertext.Length);

        return result;
    }

    /// <summary>
    /// Encrypts a string message using AES-256-GCM.
    /// </summary>
    public byte[] Encrypt(string plaintext)
    {
        return Encrypt(Encoding.UTF8.GetBytes(plaintext));
    }

    /// <summary>
    /// Decrypts data encrypted by this encryptor.
    /// Expects format: [12-byte nonce][16-byte tag][ciphertext]
    /// </summary>
    public byte[] Decrypt(byte[] encryptedData)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (encryptedData.Length < 28) // 12 + 16 minimum
            throw new ArgumentException("Encrypted data is too short to be valid.", nameof(encryptedData));

        var nonce = new byte[12];
        var tag = new byte[16];
        var ciphertext = new byte[encryptedData.Length - 28];

        Buffer.BlockCopy(encryptedData, 0, nonce, 0, 12);
        Buffer.BlockCopy(encryptedData, 12, tag, 0, 16);
        Buffer.BlockCopy(encryptedData, 28, ciphertext, 0, ciphertext.Length);

        var plaintext = new byte[ciphertext.Length];

        using var aes = new AesGcm(_key, 16);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        return plaintext;
    }

    /// <summary>
    /// Decrypts data and returns as UTF-8 string.
    /// </summary>
    public string DecryptToString(byte[] encryptedData)
    {
        return Encoding.UTF8.GetString(Decrypt(encryptedData));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            CryptographicOperations.ZeroMemory(_key);
            _disposed = true;
        }
    }
}
