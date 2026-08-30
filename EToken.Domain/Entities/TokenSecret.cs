

namespace EToken.Domain.Entities;

public class TokenSecret
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid Cif { get; set; } 
    public Guid DeviceId { get; set; }
    public byte[] EncryptedSecret { get; set; } = [];
    // public string  RsaEncryptedSecret {get;set;}="";
    public long LastAcceptedBucket { get; set; }
    public string Status { get; set; } = "active";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}