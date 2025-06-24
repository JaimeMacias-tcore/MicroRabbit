using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace MicroRabbit.Transfer.Data.Repository
{
    public class TransferRepository : ITransferRepository
    {
        private TransferDbContext _context;

        public TransferRepository(TransferDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(TransferLog transferLog)
        {
            _context.Transfers.Add(transferLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TransferLog>> GetTransfersAsync()
        {
            return await _context.Transfers.ToListAsync();
        }
    }
}
