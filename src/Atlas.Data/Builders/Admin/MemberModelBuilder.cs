using Atlas.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Atlas.Models;
using Microsoft.AspNetCore.Identity;

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

            var query = _dbContext.Members.Where(x => x.Status == StatusType.Active);

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
                TotalReplies = member.RepliesCount
            })
            .ToList();

            var countQuery = _dbContext.Members.Where(x => x.Status == StatusType.Active);

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

        public async Task<EditPageModel> BuildEditPageModelAsync(Guid id)
        {
            var result = new EditPageModel();

            var member = await _dbContext.Members
                .FirstOrDefaultAsync(x =>
                    x.Id == id && 
                    x.Status != StatusType.Deleted);

            if (member == null)
            {
                return null;
            }

            result.Member = new EditPageModel.MemberModel
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                UserId = member.UserId
            };

            var user = await _userManager.FindByIdAsync(member.UserId);

            if (user == null)
            {
                throw new DataException($"Membership user for member with id {member.Id} not found.");
            }

            foreach (var role in await _roleManager.Roles.ToListAsync())
            {
                var selected = await _userManager.IsInRoleAsync(user, role.Name);

                result.Roles.Add(new EditPageModel.RoleModel
                {
                    Name = role.Name,
                    Selected = selected
                });
            }

            return result;
        }
    }
}
