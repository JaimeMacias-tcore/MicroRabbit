using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Domain.Interfaces
{
    public interface ITransferService
    {
        Task<IEnumerable<TransferLog>> GetTransfersAsync();

        Task AddTransferAsync(TransferLog transferLog);
    }
}
