using System.Security.Cryptography;
using System.Text;
namespace EToken.Application.Commands;

public class RsaDataEncryptor
{
    public static string EncryptForClient(string plainText, string clientPublicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(clientPublicKeyPem);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        // Note: Match the padding expected by react-native-rsa-native (Pkcs1 or OaepSHA256)
        byte[] encryptedBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);

        return Convert.ToBase64String(encryptedBytes);
    }
}