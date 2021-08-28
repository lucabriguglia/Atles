using System.Threading.Tasks;

namespace Atles.Domain
{
    public interface ICommandSender
    {
        Task Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
