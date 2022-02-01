using System.Data;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain.Events;
using Atles.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers
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
