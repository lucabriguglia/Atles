using System.Threading.Tasks;

namespace Atles.Domain
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task Handle(TCommand command);
    }
}
