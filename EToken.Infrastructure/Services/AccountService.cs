using EToken.Application.Interfaces;
using EToken.Domain.Entities;

namespace EToken.Infrastructure.Services;

public class AccountService(IAccountRepository repo, IAccountNumberGenerator accountNumberGenerator) : IAccountService
{
    private readonly IAccountRepository _repo = repo;
        private readonly IAccountNumberGenerator _accountNumberGenerator = accountNumberGenerator;


    public async Task<Account?> GetAccountByIdAsync(Guid accountId, CancellationToken ct = default) =>
        await _repo.GetByIdAsync(accountId, ct);

   public async Task<Account?> GetAccountByNumberAsync(string number, CancellationToken ct = default) =>
        await _repo.GetByNumberAsync(number, ct);

    public async Task<IEnumerable<Account>> GetAccountsByCifAsync(Guid cif, CancellationToken ct = default) =>
        await _repo.GetAllByCifAsync(cif, ct);

    public async  Task<Account> RegisterAccountAsync(Guid cif, string accountType, CancellationToken ct)
    {
        
        var newAccount = new Account
        {
           
            Cif = cif,
            Status = "active",
            Type=accountType,
            Number=_accountNumberGenerator.Generate(), 
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repo.AddAsync(newAccount, ct);
        await _repo.SaveChangesAsync(ct);

        return newAccount;
    }

   
}