using System;
using System.Threading.Tasks;
using Atles.Infrastructure.Services;

namespace Atles.Infrastructure.Commands
{
    public class CommandSender : ICommandSender
    {
        private readonly IServiceProviderWrapper _serviceProvider;

        public CommandSender(IServiceProviderWrapper serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
            {
                throw new Exception($"Handler not found for command of type {typeof(TCommand)}");
            }

            await handler.Handle(command);
        }
    }
}
