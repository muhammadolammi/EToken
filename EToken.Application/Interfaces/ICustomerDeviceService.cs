using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;

public interface ICustomerDeviceService
{
    Task<CustomerDevice?> GetDeviceByIdAsync(Guid deviceId, CancellationToken ct = default);
    Task<IEnumerable<CustomerDevice>> GetDevicesByCifAsync(string cif, CancellationToken ct = default);
    Task UpdateDeviceStatusAsync(Guid deviceId,string status, CancellationToken ct = default);

    Task<CustomerDevice> RegisterDeviceAsync(string cif, string? deviceModel,  CancellationToken ct = default);
    Task<bool> RevokeDeviceAsync(Guid deviceId, CancellationToken ct = default);
}