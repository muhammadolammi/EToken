using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;

public interface ICustomerDeviceRepository
{
    Task<CustomerDevice?> GetByIdAsync(Guid deviceId, CancellationToken ct = default);
    Task<CustomerDevice?> GetActiveByCifAsync(string cif, CancellationToken ct = default);
    Task<IEnumerable<CustomerDevice>> GetAllByCifAsync(string cif, CancellationToken ct = default);
    Task AddAsync(CustomerDevice device, CancellationToken ct = default);
    void Update(CustomerDevice device);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}