using Atles.Core.Queries;
using Atles.Core.Results;
using Atles.Core.Results.Types;
using Atles.Data;
using Atles.Domain;
using Atles.Models.Admin.Users;
using Atles.Queries.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Queries.Handlers.Admin
{
    public class GetUserEditFormHandler : IQueryHandler<GetUserEditForm, EditUserPageModel>
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

        public async Task<QueryResult<EditUserPageModel>> Handle(GetUserEditForm request)
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
                return new Failure(FailureType.NotFound, "User", $"User with id {request.Id} not found.");
            }

            var result = new EditUserPageModel
            {
                User = new EditUserPageModel.UserModel
                {
                    Id = user.Id,
                    IdentityUserId = user.IdentityUserId,
                    DisplayName = user.DisplayName
                },
                Info = new EditUserPageModel.InfoModel
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

                    result.User.Roles.Add(new EditUserPageModel.RoleModel
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
