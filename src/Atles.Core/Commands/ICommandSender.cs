using System.Threading.Tasks;

namespace Atles.Core.Commands;

public interface ICommandSender
{
    Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand;
}