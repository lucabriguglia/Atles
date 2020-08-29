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

            var user = new User(command.Id,
                command.IdentityUserId,
                command.Email,
                displayName);

            if (command.Confirm)
            {
                user.Confirm();
            }

            _dbContext.Users.Add(user);

            var userIdForEvent = command.UserId == Guid.Empty 
                ? user.Id 
                : command.UserId;

            _dbContext.Events.Add(new Event(command.SiteId,
                userIdForEvent,
                EventType.Created,
                typeof(User),
                command.Id,
                new
                {
                    UserId = command.IdentityUserId,
                    command.Email,
                    DisplayName = displayName,
                    user.Status
                }));

            await _dbContext.SaveChangesAsync();
        }

        public async Task ConfirmAsync(ConfirmUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status == StatusType.Pending);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Confirm();

            var userIdForEvent = command.UserId == Guid.Empty
                ? user.Id
                : command.UserId;

            _dbContext.Events.Add(new Event(command.SiteId,
                userIdForEvent,
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

        public async Task SuspendAsync(SuspendUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

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

        public async Task ReinstateAsync(ReinstateUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Reinstate();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Reinstated,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> DeleteAsync(DeleteUser command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.Status != StatusType.Deleted);

            if (user == null)
            {
                throw new DataException($"User with Id {command.Id} not found.");
            }

            user.Delete();

            _dbContext.Events.Add(new Event(command.SiteId,
                command.UserId,
                EventType.Deleted,
                typeof(User),
                command.Id));

            await _dbContext.SaveChangesAsync();

            return user.IdentityUserId;
        }
    }
}
