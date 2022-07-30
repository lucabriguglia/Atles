using System;
using System.Threading.Tasks;
using Atles.Core.Results;
using Atles.Core.Services;

namespace Atles.Core.Commands;

public class CommandSender : ICommandSender
{
    private readonly IServiceProviderWrapper _serviceProvider;

    public CommandSender(IServiceProviderWrapper serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand
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

        return await handler.Handle(command);
    }
}