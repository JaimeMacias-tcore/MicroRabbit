using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Domain.Interfaces
{
    public interface ITransferRepository
    {
        Task<IEnumerable<TransferLog>> GetTransfersAsync();

        Task AddAsync(TransferLog transferLog);
    }
}
