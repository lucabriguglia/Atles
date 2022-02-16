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
    public class CreateUserRankHandler : ICommandHandler<CreateUserRank>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IValidator<CreateUserRank> _validator;

        public CreateUserRankHandler(AtlesDbContext dbContext, IValidator<CreateUserRank> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<IEnumerable<IEvent>> Handle(CreateUserRank command)
        {
            await _validator.ValidateCommand(command);

            var count = await _dbContext.UserRanks
                .Where(x => 
                    x.SiteId == command.SiteId && 
                    x.Status != UserRankStatusType.Deleted)
                .CountAsync();

            var sortOrder = count + 1;

            var userRank = new UserRank(command.SiteId, command.Name, command.Description, sortOrder, command.Badge, command.Role, command.Status, command.UserRankRules.ToDomainRules());

            _dbContext.UserRanks.Add(userRank);

            var @event = new UserRankCreated
            {
                Name = userRank.Name,
                Description = userRank.Description,
                SortOrder = userRank.SortOrder,
                Badge = userRank.Badge,
                Role = userRank.Role,
                Status = userRank.Status,
                UserRankRules = command.UserRankRules.ToDomainRules(),
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
