using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;

public interface IAccountRepository
{
        // Task<NameEnquiryResponse?> NameEnquiryAsync(string accountNumber, CancellationToken ct = default);

    Task<Account?> GetByIdAsync(Guid acccountId, CancellationToken ct = default);
        Task<Account?> GetByNumberAsync(string acccountNumber, CancellationToken ct = default);

    Task<IEnumerable<Account>> GetAllByCifAsync(Guid cif, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    void Update(Account account);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}