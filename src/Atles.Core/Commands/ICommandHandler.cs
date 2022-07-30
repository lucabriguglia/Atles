using System.Threading.Tasks;

namespace Atles.Core.Commands;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<CommandResult> Handle(TCommand command);
}