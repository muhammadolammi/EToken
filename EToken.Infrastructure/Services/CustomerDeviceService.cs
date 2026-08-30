using EToken.Application.Interfaces;
using EToken.Domain.Entities;

namespace EToken.Infrastructure.Services;

public class CustomerDeviceService(ICustomerDeviceRepository repo) : ICustomerDeviceService
{
    private readonly ICustomerDeviceRepository _repo = repo;

    public async Task<CustomerDevice?> GetDeviceByIdAsync(Guid deviceId, CancellationToken ct = default) =>
        await _repo.GetByIdAsync(deviceId, ct);

    public async Task<IEnumerable<CustomerDevice>> GetDevicesByCifAsync(Guid cif, CancellationToken ct = default) =>
        await _repo.GetAllByCifAsync(cif, ct);

    public async Task<CustomerDevice> RegisterDeviceAsync(Guid cif,Guid deviceIdd, string? deviceModel, CancellationToken ct = default)
    {
        // Business Rule: Check if user already has an active device
        var activeDevice = await _repo.GetActiveByCifAsync(cif, ct);
        if (activeDevice is not null)
        {
            throw new InvalidOperationException($"CIF {cif} already has an active enrolled device.");
        }

        var newDevice = new CustomerDevice
        {
            DeviceId = deviceIdd,
            Cif = cif,
            DeviceModel = deviceModel,
            Status = "inactive",
            RegisteredAt = DateTimeOffset.UtcNow
        };

        await _repo.AddAsync(newDevice, ct);
        await _repo.SaveChangesAsync(ct);

        return newDevice;
    }

    public async Task<bool> RevokeDeviceAsync(Guid deviceId, CancellationToken ct = default)
    {
        var device = await _repo.GetByIdAsync(deviceId, ct);
        if (device is null || device.Status == "revoked")
        {
            return false;
        }

        device.Status = "revoked";
        device.RevokedAt = DateTimeOffset.UtcNow;

        _repo.Update(device);
        return await _repo.SaveChangesAsync(ct) > 0;
    }

    public async Task UpdateDeviceStatusAsync(Guid deviceId,string status, CancellationToken ct = default)
    {
     var device = await _repo.GetByIdAsync(deviceId, ct);
        if (device is null || device.Status == "revoked")
        {
            return ;
        }
        device .Status = status;
         _repo.Update(device);
        
    }

    
}