
using EToken.Application.Dtos;

namespace EToken.Application.Services;


public interface ITransferService
{
    Task<TransferResponse> ProcessTransferAsync(Guid userCif, TransferRequest req, CancellationToken ct = default);
}