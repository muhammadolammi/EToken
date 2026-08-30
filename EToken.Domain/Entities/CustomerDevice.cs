

namespace EToken.Domain.Entities;

public class CustomerDevice
{
    public Guid DeviceId { get; set; }
    public Guid Cif { get; set; }
    public string? DeviceModel { get; set; }
    public string Status { get; set; } = "active";
    public DateTimeOffset RegisteredAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }
}