using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Infrastructure.Events;

namespace Atles.Infrastructure.Commands
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<IEnumerable<IEvent>> Handle(TCommand command);
    }
}
