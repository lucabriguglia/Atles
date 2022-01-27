using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Infrastructure.Events;

namespace Atles.Infrastructure.Commands
{
    public interface ICommandSender
    {
        Task<IEnumerable<IEvent>> Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
