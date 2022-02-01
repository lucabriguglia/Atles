using System.Collections.Generic;
using Atles.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using Atles.Domain.Commands;
using Atles.Domain.Events;
using Atles.Domain.Models;
using Atles.Infrastructure.Commands;
using Atles.Infrastructure.Events;

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

        public async Task<IEnumerable<IEvent>> Handle(UpdateUser command)
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

            var @event = new UserUpdated
            {
                DisplayName = user.DisplayName,
                Roles = command.Roles is { Count: > 0 } ? string.Join(", ", command.Roles) : string.Empty,
                TargetId = user.Id,
                TargetType = nameof(User),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new IEvent[] { @event };
        }
    }
}
