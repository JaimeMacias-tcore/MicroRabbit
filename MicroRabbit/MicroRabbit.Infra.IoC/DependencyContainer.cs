using MediatR;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Services;
using MicroRabbit.Banking.Data.Repository;
using MicroRabbit.Banking.Domain.CommandHandlers;
using MicroRabbit.Banking.Domain.Commands;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Infra.Bus;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Transfer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MicroRabbit.Infra.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Register the EventBus
            services.AddSingleton<IEventBus, RabbitMQBus>();
        }

        public static void RegisterBankingServices(IServiceCollection services)
        {
            //Domain banking commands
            services.AddTransient<IRequestHandler<CreateTransferCommand, bool>, TransferCommandHandler>();

            //Application services
            services.AddTransient<IAccountService, AccountService>();

            //Data
            services.AddTransient<IAccountRepository, AccountRepository>();
        }

        public static void RegisterTransferServices(IServiceCollection services)
        {
            services.AddTransient<ITransferService, TransferService>();
            services.AddTransient<ITransferRepository, TransferRepository>();
            services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();
            services.AddTransient<TransferEventHandler>();

        }
    }
}
