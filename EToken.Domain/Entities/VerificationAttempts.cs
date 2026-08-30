
namespace EToken.Domain.Entities;

public class VerificationAttempt
{
    public Guid Cif { get; set; }
    public int FailedCount { get; set; }
    public DateTimeOffset? LockedUntil { get; set; }
}