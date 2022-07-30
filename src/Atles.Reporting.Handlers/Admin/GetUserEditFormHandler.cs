using System.Threading.Tasks;
using Atles.Core.Queries;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Reporting.Handlers.Admin
{
    public class GetUserEditFormHandler : IQueryHandler<GetUserEditForm, EditPageModel>
    {
        private readonly AtlesDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public GetUserEditFormHandler(AtlesDbContext dbContext,
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<EditPageModel> Handle(GetUserEditForm request)
        {
            User user;

            if (request.Id != null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            }
            else
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == request.IdentityUserId);
            }

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
    }
}
