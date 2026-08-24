




namespace EToken.Domain.Entities;

public class VerificationAttempt(string cIF, int failedCount, DateTimeOffset lockedUntill)
{
    public string CIF { get; set; } = cIF;
    public int FailedCount { get; set; } = failedCount;
    public DateTimeOffset LockedUntill { get; set; } = lockedUntill;

} 