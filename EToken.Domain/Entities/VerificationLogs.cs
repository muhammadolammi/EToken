

namespace EToken.Domain.Entities;

public class VerificationLog
{
    public long Id { get; set; }
    public Guid Cif { get; set; } 
    public Guid DeviceId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}