using System;
using System.Data;
using System.Threading.Tasks;
using Atlas.Domain;
using Atlas.Domain.Users;
using Atlas.Domain.Users.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data.Services
{
    public class UserService : IUserService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IValidator<CreateUser> _createValidator;
        private readonly IValidator<UpdateUser> _updateValidator;

        public UserService(AtlasDbContext dbContext,
            IValidator<CreateUser> createValidator,
            IValidator<UpdateUser> updateValidator)
        {
            _dbContext = dbContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task CreateAsync(CreateUser command)
        {
            await _createValidator.ValidateCommandAsync(command);

            var displayName = await GenerateDisplayNameAsync();

            var member = new User(command.Id,
                command.IdentityUserId,
                command.Email,
                displayName);

            if (command.Confirm)
            {
                member.Confirm();
            }

            _dbContext.Users.Add(member);

            var memberIdForEvent = command.MemberId == Guid.Empty 
                ? member.Id 
                : command.MemberId;

            _dbContext.Events.Add(new Event(command.SiteId,
                memberIdForEvent,
                EventType.Created,
                typeof(User),
                command.Id,
                new
                {
                    UserId = command.IdentityUserId,
                    command.Email,
                    DisplayName = displayName,
                    member.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task ConfirmAsync(ConfirmUser command)
        {
            var member = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status == StatusType.Pending);

            if (member == null)
            {
                throw new DataException($"Member with Id {command.Id} not found.");
            }

            member.Confirm();

            var memberIdForEvent = command.MemberId == Guid.Empty
                ? member.Id
                : command.MemberId;

            _dbContext.Events.Add(new Event(command.SiteId,
                memberIdForEvent,
                EventType.Confirmed,
                typeof(User),
                command.Id));

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
                exists = await _dbContext.Users.AnyAsync(x => x.DisplayName == displayName);
                repeat++;
            }

            if (exists)
            {
                displayName = Guid.NewGuid().ToString();
            }

            return displayName;
        }

        public async Task UpdateAsync(UpdateUser command)
        {
            await _updateValidator.ValidateCommandAsync(command);

            var member = await _dbContext.Users
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
                typeof(User),
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
                    typeof(User),
                    command.Id,
                    new
                    {
                        Roles = string.Join(", ", command.Roles)
                    }));
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task SuspendAsync(SuspendUser command)
        {
            var member = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                throw new DataException($"Member with Id {command.Id} not found.");
            }

            member.Suspend();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Suspended,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }

        public async Task ReinstateAsync(ReinstateUser command)
        {
            var member = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                throw new DataException($"Member with Id {command.Id} not found.");
            }

            member.Reinstate();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.MemberId,
                EventType.Reinstated,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(DeleteUser command)
        {
            var member = await _dbContext.Users
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
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();

            return member.IdentityUserId;
        }
    }
}
