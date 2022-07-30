using System.Threading.Tasks;
using Atles.Core.Results;

namespace Atles.Core.Commands;

public interface ICommandSender
{
    Task<CommandResult> Send<TCommand>(TCommand command) where TCommand : ICommand;
}