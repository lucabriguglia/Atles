using System.Threading.Tasks;

namespace Atles.Infrastructure.Commands
{
    public interface ICommandSender
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
