using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Domain.Members;
using Atlas.Models.Admin.Members;
using Atlas.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace Atlas.Data.Builders.Admin
{
    public class MemberModelBuilder : IMemberModelBuilder
    {
        private readonly AtlasDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public MemberModelBuilder(AtlasDbContext dbContext, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IndexPageModel> BuildIndexPageModelAsync(QueryOptions options)
        {
            var result = new IndexPageModel();

            var query = _dbContext.Members.Where(x => true);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                query = query.Where(x => x.DisplayName.Contains(options.Search) || x.Email.Contains(options.Search));
            }

            var members = await query
                .OrderBy(x => x.DisplayName)
                .Skip(options.Skip)
                .Take(options.PageSize)
                .ToListAsync();

            var items = members.Select(member => new IndexPageModel.MemberModel
            {
                Id = member.Id,
                DisplayName = member.DisplayName,
                Email = member.Email,
                TotalTopics = member.TopicsCount,
                TotalReplies = member.RepliesCount,
                Status = member.Status
            })
            .ToList();

            var countQuery = _dbContext.Members.Where(x => true);

            if (!string.IsNullOrWhiteSpace(options.Search))
            {
                countQuery = countQuery.Where(x => x.DisplayName.Contains(options.Search) || x.Email.Contains(options.Search));
            }

            var totalRecords = await countQuery.CountAsync();

            result.Members = new PaginatedData<IndexPageModel.MemberModel>(items, totalRecords, options.PageSize);

            return result;
        }

        public async Task<CreatePageModel> BuildCreatePageModelAsync()
        {
            await Task.CompletedTask;

            var result = new CreatePageModel();

            return result;
        }

        public async Task<EditPageModel> BuildEditPageModelAsync(Guid memberId)
        {
            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.Id == memberId);

            return await BuildEditPageModelAsync(member);
        }

        public async Task<EditPageModel> BuildEditPageModelAsync(string userId)
        {
            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == userId);

            return await BuildEditPageModelAsync(member);
        }

        private async Task<EditPageModel> BuildEditPageModelAsync(Member member)
        {
            if (member == null)
            {
                return null;
            }

            var result = new EditPageModel
            {
                Member = new EditPageModel.MemberModel
                {
                    Id = member.Id, 
                    DisplayName = member.DisplayName
                },
                Info = new EditPageModel.InfoModel
                {
                    UserId = member.UserId, 
                    Email = member.Email, 
                    Status = member.Status
                }
            };

            var user = await _userManager.FindByIdAsync(member.UserId);

            if (user != null)
            {
                foreach (var role in await _roleManager.Roles.ToListAsync())
                {
                    var selected = await _userManager.IsInRoleAsync(user, role.Name);

                    result.Roles.Add(new EditPageModel.RoleModel
                    {
                        Name = role.Name,
                        Selected = selected
                    });
                }

                result.Info.UserName = user.UserName;
                result.Info.EmailConfirmed = user.EmailConfirmed;
                result.Info.PhoneNumber = user.PhoneNumber;
                result.Info.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                result.Info.TwoFactorEnabled = user.TwoFactorEnabled;
                result.Info.LockoutEnabled = user.LockoutEnabled;
                result.Info.AccessFailedCount = user.AccessFailedCount;
                result.Info.LockoutEnd = user.LockoutEnd;
            }

            return result;
        }

        public async Task<ActivityPageModel> BuildActivityPageModelAsync(Guid siteId, Guid memberId, QueryOptions options)
        {
            var member = await _dbContext.Members.FirstOrDefaultAsync(x => x.Id == memberId);

            if (member == null)
            {
                return null;
            }

            var result = new ActivityPageModel
            {
                Member = new ActivityPageModel.MemberModel
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName
                }
            };

            var query = _dbContext.Events.Where(x => x.SiteId == siteId && x.MemberId == memberId);

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

                if (!string.IsNullOrWhiteSpace(@event.Data) && @event.Data != "null")
                {
                    var parsedData = JObject.Parse(@event.Data);

                    var data = new Dictionary<string, string>();

                    foreach (var x in parsedData)
                    {
                        if (x.Key == nameof(@event.Id) || 
                            x.Key == nameof(@event.TargetId) ||
                            x.Key == nameof(@event.TargetType) || 
                            x.Key == nameof(@event.SiteId) ||
                            x.Key == nameof(@event.MemberId)) 
                            continue;

                        var value = !string.IsNullOrWhiteSpace(x.Value.ToString())
                            ? x.Value.ToString()
                            : "<null>";

                        data.Add(x.Key, value);
                    }

                    model.Data = data;
                }

                items.Add(model);
            }

            var totalRecords = await query.CountAsync();

            result.Events = new PaginatedData<ActivityPageModel.EventModel>(items, totalRecords, options.PageSize);

            return result;
        }
    }
}
