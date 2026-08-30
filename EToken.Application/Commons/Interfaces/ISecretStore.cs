namespace EToken.Application.Commons.Interfaces;

public interface ISecretStore
{
    Task<byte[]> Encrypt(byte[] plainTextSecret);
    Task<byte[]> Decrypt(byte[] encryptedSecret);
    }


