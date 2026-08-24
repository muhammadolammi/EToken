using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EToken.Infrastructure.Persistence.Repositories;

public class CustomerDeviceRepository : ICustomerDeviceRepository
{
    private readonly ApplicationDbContext _context;

    public CustomerDeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDevice?> GetByIdAsync(Guid deviceId, CancellationToken ct = default) =>
        await _context.CustomerDevices.FindAsync([deviceId], ct);

    public async Task<CustomerDevice?> GetActiveByCifAsync(string cif, CancellationToken ct = default) =>
        await _context.CustomerDevices
            .FirstOrDefaultAsync(d => d.Cif == cif && d.Status == "active", ct);

    public async Task<IEnumerable<CustomerDevice>> GetAllByCifAsync(string cif, CancellationToken ct = default) =>
        await _context.CustomerDevices
            .AsNoTracking()
            .Where(d => d.Cif == cif)
            .ToListAsync(ct);

    public async Task AddAsync(CustomerDevice device, CancellationToken ct = default) =>
        await _context.CustomerDevices.AddAsync(device, ct);

    public void Update(CustomerDevice device) =>
        _context.CustomerDevices.Update(device);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}