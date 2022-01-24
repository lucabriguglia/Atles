using Atles.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Models;
using Atles.Domain.Models.Users;
using Atles.Domain.Models.Users.Commands;
using Atles.Infrastructure.Commands;

namespace Atles.Domain.Handlers.Users.Commands
{
    public class UpdateUserHandler : ICommandHandler<UpdateUser>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdateUser> _validator;

        public UpdateUserHandler(AtlesDbContext dbContext, IValidator<UpdateUser> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task Handle(UpdateUser command)
        {
            await _validator.ValidateCommand(command);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.UpdateDetails(command.DisplayName);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Updated,
                typeof(User),
                command.Id,
                new
                {
                    command.DisplayName
                }));

            if (command.Roles != null && command.Roles.Count > 0)
            {
                _dbContext.Events.Add(new Event(command.SiteId,
                    command.UserId,
                    EventType.Updated,
                    typeof(User),
                    command.Id,
                    new
                    {
                        Roles = string.Join(", ", command.Roles)
                    }));
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
