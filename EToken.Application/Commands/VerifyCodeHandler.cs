namespace EToken.Application.Commands;


public class VerifyCodeHandler
{
private readonly ITokenRepository _repo;
private readonly ISecretStore _secretStore;
private readonly ITotpService _totp;
private readonly IClock _clock;
public async Task<VerifyResult> Handle(VerifyCodeCommand cmd)
{
var lockStatus = await _repo.GetLockStatus(cmd.Cif);
if (lockStatus.IsLocked)
return VerifyResult.LockedOut(lockStatus.LockedUntil);
var record = await _repo.GetActiveSecret(cmd.Cif, cmd.DeviceId);
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
await _repo.IncrementFailedAttempts(cmd.Cif);
await _repo.LogAttempt(cmd.Cif, cmd.DeviceId, cmd.ActionType, success: false);
return VerifyResult.Invalid("invalid_code");
}
await _repo.UpdateLastAcceptedBucket(record.Id, currentBucket);
await _repo.ResetFailedAttempts(cmd.Cif);
await _repo.LogAttempt(cmd.Cif, cmd.DeviceId, cmd.ActionType, success: true);
return VerifyResult.Success();
}
}