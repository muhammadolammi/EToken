using EToken.Application.Commands;
using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;

public interface ITokenService
{
    Task<TokenProvisionResult> ProvisionTokenAsync(Guid cif, Guid deviceId);
    Task<GetTokenRecord?> GetByDeviceIdAsync( Guid deviceId);

    Task<VerifyResult> VerifyCodeAsync(Guid cif, Guid deviceId, string code, string actionType);
    Task RevokeTokenAsync(Guid cif, Guid deviceId);

}

public record TokenProvisionResult(
    Guid TokenSecretId,
    Byte[] EncryptedSecret
);