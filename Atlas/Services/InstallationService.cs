using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Services
{
    public class InstallationService : IInstallationService
    {
        private readonly AtlasDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public InstallationService(AtlasDbContext dbContext, 
            IConfiguration configuration, 
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task EnsureAdminUserInitializedAsync()
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (await roleManager.RoleExistsAsync("Admin") == false)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (_configuration["CreateDefaultAdminUser"].ToLowerInvariant() != "true")
            {
                return;
            }

            if (await userManager.FindByEmailAsync(_configuration["DefaultAdminUserEmail"]) != null)
            {
                return;
            }

            var user = new IdentityUser
            {
                Email = _configuration["DefaultAdminUserEmail"],
                UserName = _configuration["DefaultAdminUserName"]
            };

            var userResult = await userManager.CreateAsync(user, _configuration["DefaultAdminUserPassword"]);

            if (!userResult.Succeeded)
            {
                return;
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await userManager.ConfirmEmailAsync(user, code);

            await userManager.AddToRoleAsync(user, "Admin");

            var member = new Member(user.Id);
            _dbContext.Members.Add(member);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EnsureDefaultSiteInitializedAsync()
        {
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default");

            if (site != null)
            {
                return;
            }

            site = new Site("Default", "My Website");
            _dbContext.Sites.Add(site);

            var permissionSet = new PermissionSet(site.Id, "Default", new List<Permission>());
            _dbContext.PermissionSets.Add(permissionSet);

            var forumGroup = new ForumGroup(site.Id, "General", 1);
            _dbContext.ForumGroups.Add(forumGroup);

            var forum = new Forum(forumGroup.Id, "Welcome", 1);
            _dbContext.Forums.Add(forum);

            await _dbContext.SaveChangesAsync();
        }
    }
}