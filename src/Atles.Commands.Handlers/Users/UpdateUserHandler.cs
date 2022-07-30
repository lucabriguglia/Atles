using System.Data;
using Atles.Commands.Users;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Core.Results;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Commands.Handlers.Users
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

        public async Task<CommandResult> Handle(UpdateUser command)
        {
            await _validator.ValidateCommand(command);

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.UpdateUserId);

            if (user == null)
            {
                throw new DataException($"User with Id {command.UpdateUserId} not found.");
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

            return new Success(new IEvent[] { @event });
        }
    }
}
