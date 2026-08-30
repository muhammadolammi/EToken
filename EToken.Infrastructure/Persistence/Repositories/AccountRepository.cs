using EToken.Application.Interfaces;
using EToken.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EToken.Infrastructure.Persistence.Repositories;

public class AccountRepository(ApplicationDbContext context) : IAccountRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Account?> GetByIdAsync(Guid acccountId, CancellationToken ct = default) =>
        await _context.Accounts.FindAsync([acccountId], ct);
    public async Task<Account?> GetByNumberAsync(string number, CancellationToken ct = default) =>
        await _context.Accounts.FirstOrDefaultAsync(a => a.Number==number, ct);

    public async Task<IEnumerable<Account>> GetAllByCifAsync(Guid cif, CancellationToken ct = default) =>
        await _context.Accounts
            .AsNoTracking()
            .Where(d => d.Cif == cif)
            .ToListAsync(ct);

    public async Task AddAsync(Account account, CancellationToken ct = default) =>
        await _context.Accounts.AddAsync(account, ct);

    public void Update(Account account) =>
        _context.Accounts.Update(account);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);



}