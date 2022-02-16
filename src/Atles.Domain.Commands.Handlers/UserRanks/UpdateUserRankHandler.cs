using System.Data;
using Atles.Core.Commands;
using Atles.Core.Events;
using Atles.Data;
using Atles.Domain.Commands.UserRanks;
using Atles.Domain.Events.UserRanks;
using Atles.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atles.Domain.Commands.Handlers.UserRanks
{
    public class UpdateUserRankHandler : ICommandHandler<UpdateUserRank>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<UpdateUserRank> _validator;

        public UpdateUserRankHandler(AtlesDbContext dbContext, IValidator<UpdateUserRank> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<IEnumerable<IEvent>> Handle(UpdateUserRank command)
        {
            await _validator.ValidateCommand(command);

            var userRank = await _dbContext.UserRanks
                .FirstOrDefaultAsync(x =>
                    x.SiteId == command.SiteId &&
                    x.Id == command.Id &&
                    x.Status != UserRankStatusType.Deleted);

            if (userRank == null)
            {
                throw new DataException($"User rank with Id {command.Id} not found.");
            }

            userRank.UpdateDetails(command.Name, command.Description, command.Badge, command.Role, command.Status, command.UserRankRules.ToDomainRules());

            var @event = new UserRankUpdated
            {
                Name = userRank.Name,
                Description = userRank.Description,
                Badge = userRank.Badge,
                Role = userRank.Role,
                Status = userRank.Status,
                TargetId = userRank.Id,
                TargetType = nameof(UserRank),
                SiteId = command.SiteId,
                UserId = command.UserId
            };

            _dbContext.Events.Add(@event.ToDbEntity());

            await _dbContext.SaveChangesAsync();

            return new IEvent[] { @event };
        }
    }
}
