using System.Security.Cryptography;
using EToken.Application.Commons.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EToken.Infrastructure.Security;

public class AesGcmSecretStore : ISecretStore
{
    private readonly byte[] _masterKey;
    private static readonly int NonceSize = AesGcm.NonceByteSizes.MaxSize; // 12 bytes
    private static readonly int TagSize = AesGcm.TagByteSizes.MaxSize;     // 16 bytes

    public AesGcmSecretStore(IConfiguration config)
    {
        // 256-bit (32-byte) key configured in appsettings.json or environment variable
        var base64Key = config["KmsSettings:MasterKey"] 
            ?? throw new InvalidOperationException("KmsSettings:MasterKey is missing.");
        
        _masterKey = Convert.FromBase64String(base64Key);
        if (_masterKey.Length != 32)
        {
            throw new ArgumentException("MasterKey must be a 32-byte (256-bit) Base64 string.");
        }
    }

    public Task<byte[]> Encrypt(byte[] plainTextSecret)
    {
        // 1. Generate unique 12-byte nonce for every encryption
        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] ciphertext = new byte[plainTextSecret.Length];
        byte[] tag = new byte[TagSize];

        // 2. Perform AES-GCM encryption
        using (var aesGcm = new AesGcm(_masterKey, TagSize))
        {
            aesGcm.Encrypt(nonce, plainTextSecret, ciphertext, tag);
        }

        // 3. Format payload: [12-byte Nonce] + [16-byte Tag] + [Ciphertext]
        byte[] combinedPayload = new byte[NonceSize + TagSize + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, combinedPayload, 0, NonceSize);
        Buffer.BlockCopy(tag, 0, combinedPayload, NonceSize, TagSize);
        Buffer.BlockCopy(ciphertext, 0, combinedPayload, NonceSize + TagSize, ciphertext.Length);

        return Task.FromResult(combinedPayload);
    }

    public Task<byte[]> Decrypt(byte[] encryptedSecret)
    {
        if (encryptedSecret.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("Ciphertext payload is too short or malformed.");
        }

        // 1. Extract Nonce, Tag, and Ciphertext from the binary payload
        byte[] nonce = new byte[NonceSize];
        byte[] tag = new byte[TagSize];
        int ciphertextSize = encryptedSecret.Length - NonceSize - TagSize;
        byte[] ciphertext = new byte[ciphertextSize];
        byte[] plaintext = new byte[ciphertextSize];

        Buffer.BlockCopy(encryptedSecret, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(encryptedSecret, NonceSize, tag, 0, TagSize);
        Buffer.BlockCopy(encryptedSecret, NonceSize + TagSize, ciphertext, 0, ciphertextSize);

        // 2. Decrypt & authenticate
        using (var aesGcm = new AesGcm(_masterKey, TagSize))
        {
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
        }

        return Task.FromResult(plaintext);
    }
}