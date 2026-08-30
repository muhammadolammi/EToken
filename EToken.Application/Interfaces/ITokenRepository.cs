
using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;
 
public interface ITokenRepository
{
    Task<(bool IsLocked, DateTimeOffset? LockedUntil)> GetLockStatus(Guid cif);
    Task<GetTokenRecord?> GetByDeviceIdAsync( Guid deviceId);
    Task IncrementFailedAttempts(Guid cif);
    Task ResetFailedAttempts(Guid cif);
    Task UpdateLastAcceptedBucket(Guid secretId, long bucket);
    Task LogAttempt(Guid cif, Guid deviceId, string actionType, bool success);


    Task AddAsync(TokenSecret token, CancellationToken ct = default);

}

public record GetTokenRecord(Guid Id, byte[] EncryptedSecret, long LastAcceptedBucket);