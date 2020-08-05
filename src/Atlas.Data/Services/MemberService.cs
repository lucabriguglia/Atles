using System;
using System.Data;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Members;
using Atlas.Domain.Members.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class MemberService : IMemberService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IValidator<CreateMember> _createValidator;
        private readonly IValidator<UpdateMember> _updateValidator;

        public MemberService(AtlasDbContext dbContext,
            IValidator<CreateMember> createValidator,
            IValidator<UpdateMember> updateValidator)
        {
            _dbContext = dbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateMember command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var displayName = await GenerateDisplayNameAsync();

            var member = new Member(command.Id,
                command.UserId,
                command.Email,
                displayName);

            _dbContext.Members.Add(member);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Created,
                typeof(Member),
                command.Id,
                new
                {
                    command.UserId,
                    command.Email,
                    DisplayName = displayName
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> GenerateDisplayNameAsync()
        {
            var displayName = string.Empty;
            var exists = true;
            var repeat = 0;
            var random = new Random();

            while (exists && repeat < 5)
            {
                displayName = $"User{random.Next(100000)}";
                exists = await _dbContext.Members.AnyAsync(x => x.DisplayName == displayName);
                repeat++;
                if (exists)
                {
                    displayName = string.Empty;
                }
            }

            if (displayName == string.Empty)
            {
                displayName = Guid.NewGuid().ToString();
            }

            return displayName;
        }

        public async Task UpdateAsync(UpdateMember command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id);

            if (member == null)
            {
                throw new DataException($"Member with Id {command.Id} not found.");
            }

            member.UpdateDetails(command.DisplayName);

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Updated,
                typeof(Member),
                command.Id,
                new
                {
                    command.DisplayName
                }));

            if (command.Roles != null && command.Roles.Count > 0)
            {
                _dbContext.Events.Add(new Event(command.SiteId,
                    command.MemberId,
                    EventType.Updated,
                    typeof(Member),
                    command.Id,
                    new
                    {
                        Roles = string.Join(", ", command.Roles)
                    }));
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(DeleteMember command)
        {
            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                throw new DataException($"Member with Id {command.Id} not found.");
            }

            member.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Deleted,
                typeof(Member),
                command.Id));

            await _dbContext.SaveChangesAsync();

            return member.UserId;
        }
    }
}
