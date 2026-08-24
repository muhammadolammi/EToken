
using System.Numerics;

namespace EToken.Domain.Entities;

public class TokenSecret(Guid id, Guid deviceId, string cIF, string encryptedSecret, BigInteger lastAcceptedBucket, string status, DateTimeOffset createddAt)
{
    public Guid Id { get; set; } = id;
    public Guid DeviceId { get; set; } = deviceId;

    public string CIF { get; set; } = cIF;
    public string EncryptedSecret { get; set; } = encryptedSecret;

    public BigInteger LastAcceptedBucket { get; set; } = lastAcceptedBucket;
    public string Status { get; set; } = status;
    public DateTimeOffset CreateddAt { get; set; } = createddAt;

} 