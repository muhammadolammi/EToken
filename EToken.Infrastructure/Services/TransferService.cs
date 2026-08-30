using EToken.Application.Commands;
using EToken.Application.Dtos;
using EToken.Application.Services;
using EToken.Infrastructure.Persistence;
using MediatR;
using EToken.Domain.Entities;
namespace EToken.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
public class TransferService(
    ApplicationDbContext context,
    IMediator mediator) : ITransferService
{
    
    public async Task<TransferResponse> ProcessTransferAsync(Guid userCif, TransferRequest req, CancellationToken ct = default)
    {
        if (req.Amount <= 0)
            throw new InvalidOperationException("Transfer amount must be greater than zero.");

        // 1. Verify E-Token directly via MediatR
        var tokenValidation = await mediator.Send(
            new VerifyCodeCommand(userCif, req.DeviceId, req.ETokenCode, "transfer"), 
            ct
        );

        if (!tokenValidation.IsValid)
        {
            throw new UnauthorizedAccessException(tokenValidation.Reason ?? "Invalid or expired E-Token.");
        }

        // 2. Perform Atomic Balance Transfer in DB Transaction
        using var dbTransaction = await context.Database.BeginTransactionAsync(ct);
        try
        {
            var source = await context.Accounts.FirstOrDefaultAsync(a => a.Id == req.SourceAccountId && a.Cif == userCif, ct);

            if (source is null)
                throw new KeyNotFoundException("Source account not found or does not belong to user.");

            if (source.Balance < req.Amount)
                throw new InvalidOperationException("Insufficient funds.");

            var destination = await context.Accounts
                .FirstOrDefaultAsync(a => a.Number == req.DestinationAccountNumber && a.Status == "active", ct);

            if (destination is null)
                throw new KeyNotFoundException("Destination account not found or inactive.");

            if (source.Id == destination.Id)
                throw new InvalidOperationException("Cannot transfer to the same account.");

            // 3. Debit & Credit
            source.Balance -= req.Amount;
            destination.Balance += req.Amount;

            // 4. Record Transaction History
            var txn = new Transaction
            {
                Id = Guid.NewGuid(),
                Reference = $"TXN-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Random.Shared.Next(1000, 9999)}",
                SourceAccountId = source.Id,
                DestinationAccountId = destination.Id,
                Amount = req.Amount,
                Narration = req.Narration,
                Status = "successful",
                CreatedAt = DateTimeOffset.UtcNow
            };

            await context.Transactions.AddAsync(txn, ct);
            await context.SaveChangesAsync(ct);

            await dbTransaction.CommitAsync(ct);

            return new TransferResponse(txn.Id, txn.Reference, txn.Amount, txn.Status, txn.CreatedAt);
        }
        catch
        {
            await dbTransaction.RollbackAsync(ct);
            throw;
        }
    }
}