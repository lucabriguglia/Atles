using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Core.Events;

namespace Atles.Core.Commands;

public interface ICommandSender
{
    Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand;
}