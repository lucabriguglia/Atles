using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Atles.Domain
{
    public class CommandSender : ICommandSender
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandSender(IServiceProvider serviceProvider)
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
