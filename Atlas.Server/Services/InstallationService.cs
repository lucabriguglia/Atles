using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Categories.Events;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Events;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Events;
using Atlas.Domain.Sites;
using Atlas.Domain.Sites.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atlas.Server.Services
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

            var member = new Member(user.Id, "Admin");
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

            // Site
            site = new Site("Default", "My Website");
            _dbContext.Sites.Add(site);
            _dbContext.Events.Add(new Event(new SiteCreated
            {
                SiteId = site.Id,
                MemberId = null,
                TargetId = site.Id,
                TargetType = typeof(Site).Name,
                Name = site.Name,
                Title = site.Title
            }));

            // Permission Set
            var permissionSet = new PermissionSet(site.Id, "Default", new List<Permission>());
            _dbContext.PermissionSets.Add(permissionSet);
            _dbContext.Events.Add(new Event(new PermissionSetCreated
            {
                SiteId = permissionSet.SiteId,
                MemberId = null,
                TargetId = permissionSet.Id,
                TargetType = typeof(PermissionSet).Name,
                Name = site.Name
            }));

            // Category
            var category = new Category(site.Id, "General", 1);
            _dbContext.Categories.Add(category);
            _dbContext.Events.Add(new Event(new CategoryCreated
            {
                SiteId = category.SiteId,
                MemberId = null,
                TargetId = category.Id,
                TargetType = typeof(Category).Name,
                Name = category.Name,
                PermissionSetId = category.PermissionSetId,
                SortOrder = category.SortOrder
            }));

            // Forum
            var forum = new Forum(category.Id, "Welcome", 1);
            _dbContext.Forums.Add(forum);
            _dbContext.Events.Add(new Event(new ForumCreated
            {
                SiteId = category.SiteId,
                MemberId = null,
                TargetId = forum.Id,
                TargetType = typeof(Forum).Name,
                Name = forum.Name,
                PermissionSetId = forum.PermissionSetId,
                SortOrder = forum.SortOrder
            }));

            await _dbContext.SaveChangesAsync();
        }
    }
}