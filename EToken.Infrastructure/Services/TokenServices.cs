using System.Security.Cryptography;
using EToken.Application.Commands;
using EToken.Application.Commons.Interfaces;
using EToken.Application.Interfaces;
using EToken.Application.Services;
using EToken.Domain.Entities;
using Microsoft.Extensions.Logging;
using OtpNet;

namespace EToken.Infrastructure.Services;

public class TokenService(
    ITokenRepository tokenRepo,
    ISecretStore secretStore,
    ITotpService totpService,
    IClock clock,
  ILogger<TokenService> logger
    
    ) : ITokenService
{
    private const int SecretLengthBytes = 20; // 160-bit secret for standard HMAC-SHA1

    /// <summary>
    /// Generates a new cryptographic TOTP seed, encrypts it via KMS/SecretStore, and persists it.
    /// </summary>
    public async Task<TokenProvisionResult> ProvisionTokenAsync(Guid cif, Guid deviceId)
    {
        // 1. Generate cryptographically secure random bytes for TOTP seed
        byte[] rawSecret = RandomNumberGenerator.GetBytes(SecretLengthBytes);

        // 2. Encrypt seed for secure storage
        byte[] encryptedSecret = await secretStore.Encrypt(rawSecret); 
       
        // 3. Persist new secret via repository / context
        var tokenSecret = new TokenSecret
        {
            Id = Guid.NewGuid(),
            Cif = cif,
            DeviceId = deviceId,
            EncryptedSecret = encryptedSecret,
            LastAcceptedBucket = 0,
            Status = "active",
            CreatedAt = clock.UtcNow
        };

        await tokenRepo.AddAsync(tokenSecret);

        // 4. Format for mobile authenticator / QR provisioning
        // string secretBase32 = Base32Encoding.ToString(rawSecret);
        // string qrCodeUri = $"otpauth://totp/EToken:{cif}?secret={secretBase32}&issuer=EToken&period=30&digits=6";

        return new TokenProvisionResult(tokenSecret.Id, encryptedSecret);
    }

    /// <summary>
    /// Validates an incoming 6-digit TOTP code against replay, drift, and lockout constraints.
    /// </summary>
    public async Task<VerifyResult> VerifyCodeAsync(Guid cif, Guid deviceId, string code, string actionType)
    {
        // 1. Check lockout status
        var lockStatus = await tokenRepo.GetLockStatus(cif);
        if (lockStatus.IsLocked)
        {
            return VerifyResult.LockedOut(lockStatus.LockedUntil);
        }

        // 2. Fetch active secret record
        var record = await tokenRepo.GetByDeviceIdAsync( deviceId);
        if (record is null)
        {
            return VerifyResult.Invalid("no_active_token");
        }

        // 3. Decrypt stored secret ciphertext
        byte[] secret = await secretStore.Decrypt(record.EncryptedSecret);

        // 4. Anti-Replay check: verify 30-second epoch bucket has advanced
        var now = clock.UtcNow;
        long currentBucket = now.ToUnixTimeSeconds() / 30;
        if (currentBucket <= record.LastAcceptedBucket)
        {
            return VerifyResult.Invalid("replay");
        }

        // 5. Verify TOTP code against time window
        bool isValid = totpService.Verify(secret, code, now);
        if (!isValid)
        {
            await tokenRepo.IncrementFailedAttempts(cif);
            await tokenRepo.LogAttempt(cif, deviceId, actionType, success: false);
            return VerifyResult.Invalid("invalid_code");
        }

        // 6. On success: advance replay window bucket and reset failed counters
        await tokenRepo.UpdateLastAcceptedBucket(record.Id, currentBucket);
        await tokenRepo.ResetFailedAttempts(cif);
        await tokenRepo.LogAttempt(cif, deviceId, actionType, success: true);

        return VerifyResult.Success();
    }

    /// <summary>
    /// Revokes active token secrets associated with a device.
    /// </summary>
    public async Task RevokeTokenAsync(Guid cif, Guid deviceId)
    {
        // Add revocation logic or update status to 'revoked' via repo
        await Task.CompletedTask;
    }

    public async Task<GetTokenRecord?> GetByDeviceIdAsync(Guid deviceId)
    {
             var res=   await tokenRepo.GetByDeviceIdAsync(deviceId);
             return res;

    }
}