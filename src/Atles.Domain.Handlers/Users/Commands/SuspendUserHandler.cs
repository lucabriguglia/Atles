using Atles.Data;
using Microsoft.EntityFrameworkCore;
using OpenCqrs.Commands;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;

namespace Atles.Domain.Handlers.Users.Commands
{
    public class SuspendUserHandler : ICommandHandler<SuspendUser>
    {
        private readonly AtlesDbContext _dbContext;

        public SuspendUserHandler(AtlesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(SuspendUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != UserStatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Suspend();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Suspended,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }
    }
}
