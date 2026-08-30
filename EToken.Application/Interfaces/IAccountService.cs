using EToken.Domain.Entities;

namespace EToken.Application.Interfaces;

public interface IAccountService
{
    Task<Account?> GetAccountByIdAsync(Guid deviceId, CancellationToken ct = default);
        Task<Account?> GetAccountByNumberAsync(string number, CancellationToken ct = default);

    Task<IEnumerable<Account>> GetAccountsByCifAsync(Guid cif, CancellationToken ct = default);

    Task<Account> RegisterAccountAsync(Guid cif,  string accountType,  CancellationToken ct = default);
}