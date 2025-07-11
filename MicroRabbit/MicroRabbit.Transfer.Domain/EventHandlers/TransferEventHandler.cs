﻿using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;

namespace MicroRabbit.Transfer.Domain.EventHandlers
{
    public class TransferEventHandler : IEventHandler<TransferCreatedEvent>
    {
        private readonly ITransferService _transferService;

        public TransferEventHandler(ITransferService transferService)
        {
            _transferService = transferService ?? throw new ArgumentNullException(nameof(transferService));
        }

        public async Task Handle(TransferCreatedEvent @event)
        {
            await _transferService.AddTransferAsync(new TransferLog
            {
                FromAccount = @event.From,
                ToAccount = @event.To,
                Amount = @event.Amount
            });
        }
    }
}
