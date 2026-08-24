

namespace EToken.Domain.Entities;

public class CustomerDevice
{
    public Guid DeviceId { get; set; }
    public string Cif { get; set; } = string.Empty;
    public string? DeviceModel { get; set; }
    public string Status { get; set; } = "active";
    public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
}