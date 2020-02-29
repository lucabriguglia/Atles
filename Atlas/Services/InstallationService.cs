using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Atlas.Services
{
    public class InstallationService : IInstallationService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AtlasDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public InstallationService(RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager, 
            AtlasDbContext dbContext, 
            IConfiguration configuration)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task EnsureAdminUserInitializedAsync()
        {
            if (await _roleManager.RoleExistsAsync("Admin") == false)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (_configuration["CreateDefaultAdminUser"].ToLowerInvariant() != "true")
            {
                return;
            }

            if (await _userManager.FindByEmailAsync(_configuration["DefaultAdminUserEmail"]) != null)
            {
                return;
            }

            var user = new IdentityUser
            {
                Email = _configuration["DefaultAdminUserEmail"],
                UserName = _configuration["DefaultAdminUserName"]
            };

            var userResult = await _userManager.CreateAsync(user, _configuration["DefaultAdminUserPassword"]);

            if (!userResult.Succeeded)
            {
                return;
            }

            await _userManager.AddToRoleAsync(user, "Admin");

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

            var forumGroup = new ForumGroup(site.Id, "General", 1);
            _dbContext.ForumGroups.Add(forumGroup);

            var forum = new Forum(forumGroup.Id, "Welcome", 1);
            _dbContext.Forums.Add(forum);

            await _dbContext.SaveChangesAsync();
        }
    }
}