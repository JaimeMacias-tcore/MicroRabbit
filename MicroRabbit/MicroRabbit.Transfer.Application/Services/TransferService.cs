using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRepository _transferRepository;

        public TransferService(ITransferRepository transferRepository, IEventBus bus)
        {
            _transferRepository = transferRepository ?? throw new ArgumentNullException(nameof(transferRepository));
        }

        public async Task AddTransferAsync(TransferLog transferLog)
        {
            await _transferRepository.AddAsync(transferLog);
        }

        public async Task<IEnumerable<TransferLog>> GetTransfersAsync()
        {
            return await _transferRepository.GetTransfersAsync();
        }        
    }
}
