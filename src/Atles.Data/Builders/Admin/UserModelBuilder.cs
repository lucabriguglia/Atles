using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atles.Domain.Users;
using Atles.Models;
using Atles.Models.Admin.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Atles.Data.Builders.Admin
{
    public class UserModelBuilder : IUserModelBuilder
    {
        private readonly AtlesDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserModelBuilder(AtlesDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<CreatePageModel> BuildCreatePageModelAsync()
        {
            await Task.CompletedTask;

            var result = new CreatePageModel();

            return result;
        }

        public async Task<EditPageModel> BuildEditPageModelAsync(Guid memberId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == memberId);

            return await BuildEditPageModelAsync(user);
        }

        public async Task<EditPageModel> BuildEditPageModelAsync(string identityUserId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == identityUserId);

            return await BuildEditPageModelAsync(user);
        }

        private async Task<EditPageModel> BuildEditPageModelAsync(User user)
        {
            if (user == null)
            {
                return null;
            }

            var result = new EditPageModel
            {
                User = new EditPageModel.UserModel
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName
                },
                Info = new EditPageModel.InfoModel
                {
                    UserId = user.IdentityUserId,
                    Email = user.Email,
                    Status = user.Status
                }
            };

            var identityUser = await _userManager.FindByIdAsync(user.IdentityUserId);

            if (identityUser != null)
            {
                foreach (var role in await _roleManager.Roles.ToListAsync())
                {
                    var selected = await _userManager.IsInRoleAsync(identityUser, role.Name);

                    result.Roles.Add(new EditPageModel.RoleModel
                    {
                        Name = role.Name,
                        Selected = selected
                    });
                }

                result.Info.UserName = identityUser.UserName;
                result.Info.EmailConfirmed = identityUser.EmailConfirmed;
                result.Info.PhoneNumber = identityUser.PhoneNumber;
                result.Info.PhoneNumberConfirmed = identityUser.PhoneNumberConfirmed;
                result.Info.TwoFactorEnabled = identityUser.TwoFactorEnabled;
                result.Info.LockoutEnabled = identityUser.LockoutEnabled;
                result.Info.AccessFailedCount = identityUser.AccessFailedCount;
                result.Info.LockoutEnd = identityUser.LockoutEnd;
            }

            return result;
        }

        public async Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid userId, QueryOptions options)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return null;
            }

            var result = new ActivityPageModel
            {
                User = new ActivityPageModel.UserModel
                {
                    Id = user.Id,
                    DisplayName = user.DisplayName
                }
            };

            var query = _dbContext.Events.Where(x => x.SiteId == siteId && x.UserId == userId);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                query = query.Where(x => x.Type.Contains(options.Search) ||
                                         x.TargetType.Contains(options.Search) ||
                                         x.Data.Contains(options.Search));
            }

            var events = await query
                .OrderByDescending(x => x.TimeStamp)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = new List<ActivityPageModel.EventModel>();

            foreach (var @event in events)
            {
                var model = new ActivityPageModel.EventModel
                {
                    Id = @event.Id,
                    Type = @event.Type,
                    TargetId = @event.TargetId,
                    TargetType = @event.TargetType,
                    TimeStamp = @event.TimeStamp
                };

                var data = new Dictionary<string, string>
                {
                    {nameof(@event.TargetId), @event.TargetId.ToString()}
                };

                if (!string.IsNullOrWhiteSpace(@event.Data) && @event.Data != "null")
                {
                    var parsedData = JObject.Parse(@event.Data);

                    foreach (var x in parsedData)
                    {
                        if (x.Key == nameof(@event.Id) ||
                            x.Key == nameof(@event.TargetId) ||
                            x.Key == nameof(@event.TargetType) ||
                            x.Key == nameof(@event.SiteId) ||
                            x.Key == nameof(@event.UserId))
                            continue;

                        var value = !string.IsNullOrWhiteSpace(x.Value.ToString())
                            ? x.Value.ToString()
                            : "<null>";

                        data.Add(x.Key, value);
                    }
                }

                model.Data = data;

                items.Add(model);
            }

            var totalRecords = await query.CountAsync();

            result.Events = new PaginatedData<ActivityPageModel.EventModel>(items, totalRecords, options.PageSize);

            return result;
        }
    }
}
