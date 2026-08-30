using MediatR;
namespace EToken.Application.Commands;
using System.Threading.Tasks;
using EToken.Application.Commons.Interfaces;
using EToken.Application.Interfaces;
using EToken.Application.Services;
using MediatR;
public record VerifyResult(bool IsValid, string? Reason = null, DateTimeOffset? LockedUntil = null)
{
    public static VerifyResult Success() => new(true);
    public static VerifyResult Invalid(string reason) => new(false, Reason: reason);
    public static VerifyResult LockedOut(DateTimeOffset? until) => new(false, Reason: "account_locked", LockedUntil: until);
}
public record VerifyCodeCommand(
    Guid Cif,
    Guid DeviceId,
    string Code,
    string ActionType
) : IRequest<VerifyResult>;

public class VerifyCodeHandler(
    ITokenRepository _tokenRepo,
    ISecretStore _secretStore,
    ITotpService _totp,
    IClock _clock) : IRequestHandler<VerifyCodeCommand, VerifyResult>
{

public async Task<VerifyResult> Handle(VerifyCodeCommand cmd, CancellationToken ct)
{
var (IsLocked, LockedUntil) = await _tokenRepo.GetLockStatus(cmd.Cif);
if (IsLocked)
return VerifyResult.LockedOut(LockedUntil);
var record = await _tokenRepo.GetByDeviceIdAsync( cmd.DeviceId);
if (record is null)
return VerifyResult.Invalid("no_active_token");
var secret = await _secretStore.Decrypt(record.EncryptedSecret);
var now = _clock.UtcNow;
var currentBucket = now.ToUnixTimeSeconds() / 30;
if (currentBucket <= record.LastAcceptedBucket)
return VerifyResult.Invalid("replay");
var valid = _totp.Verify(secret, cmd.Code, now);
if (!valid)
{
await _tokenRepo.IncrementFailedAttempts(cmd.Cif);
await _tokenRepo.LogAttempt(cmd.Cif, cmd.DeviceId, cmd.ActionType, success: false);
return VerifyResult.Invalid("invalid_code");
}
await _tokenRepo.UpdateLastAcceptedBucket(record.Id, currentBucket);
await _tokenRepo.ResetFailedAttempts(cmd.Cif);
await _tokenRepo.LogAttempt(cmd.Cif, cmd.DeviceId, cmd.ActionType, success: true);
return VerifyResult.Success();
}

    
}