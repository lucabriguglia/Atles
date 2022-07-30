using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Core.Events;

namespace Atles.Core.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<CommandResult> Handle(TCommand command);
}