using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EToken.Infrastructure.Persistence.Repositories;

public class TokenRepository(ApplicationDbContext context) : ITokenRepository
{
    private const int MaxFailedAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Checks if the customer account is currently locked due to too many failed TOTP attempts.
    /// </summary>
    public async Task<(bool IsLocked, DateTimeOffset? LockedUntil)> GetLockStatus(Guid cif)
    {
        var attempt = await context.VerificationAttempts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Cif == cif);

        if (attempt?.LockedUntil is not null)
        {
            // If the lockout timestamp is in the future, the user is locked out
            if (attempt.LockedUntil.Value > DateTimeOffset.UtcNow)
            {
                return (true, attempt.LockedUntil);
            }
        }

        return (false, null);
    }

    /// <summary>
    /// Fetches the active TOTP seed secret for a specific customer and device.
    /// </summary>
    public async Task<GetTokenRecord?> GetByDeviceIdAsync( Guid deviceId)
    {
        var secret = await context.TokenSecrets
            .AsNoTracking()
            .Where(s =>   s.DeviceId == deviceId )
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (secret is null) return null;

        return new GetTokenRecord(secret.Id, secret.EncryptedSecret, secret.LastAcceptedBucket);
    }

    /// <summary>
    /// Increments the consecutive failed attempts count.
    /// Automatically applies a 15-minute lockout if threshold (5) is reached.
    /// </summary>
    public async Task IncrementFailedAttempts(Guid cif)
    {
        var attempt = await context.VerificationAttempts.FindAsync(cif);

        if (attempt is null)
        {
            attempt = new VerificationAttempt
            {
                Cif = cif,
                FailedCount = 1,
                LockedUntil = null
            };
            context.VerificationAttempts.Add(attempt);
        }
        else
        {
            attempt.FailedCount++;
            if (attempt.FailedCount >= MaxFailedAttempts)
            {
                attempt.LockedUntil = DateTimeOffset.UtcNow.Add(LockoutDuration);
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Resets the failed attempts counter to 0 and clears any active lockout upon successful validation.
    /// </summary>
    public async Task ResetFailedAttempts(Guid cif)
    {
        var attempt = await context.VerificationAttempts.FindAsync(cif);

        if (attempt is not null)
        {
            attempt.FailedCount = 0;
            attempt.LockedUntil = null;
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Updates the last accepted 30-second time bucket to prevent OTP replay attacks.
    /// </summary>
    public async Task UpdateLastAcceptedBucket(Guid secretId, long bucket)
    {
        await context.TokenSecrets
            .Where(s => s.Id == secretId)
            .ExecuteUpdateAsync(setter => setter.SetProperty(s => s.LastAcceptedBucket, bucket));
    }

    /// <summary>
    /// Writes an immutable audit log entry for every verification attempt.
    /// </summary>
    public async Task LogAttempt(Guid cif, Guid deviceId, string actionType, bool success)
    {
        var log = new VerificationLog
        {
            Cif = cif,
            DeviceId = deviceId,
            ActionType = actionType,
            Result = success ? "success" : "failed",
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.VerificationLogs.Add(log);
        await context.SaveChangesAsync();
    }

    public async Task AddAsync(TokenSecret token, CancellationToken ct = default)
    {
            await context.TokenSecrets.AddAsync(token, ct);
await context.SaveChangesAsync(ct);
    }

   
}