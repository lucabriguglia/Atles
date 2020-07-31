using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Categories.Events;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Events;
using Atlas.Domain.Members;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using Atlas.Domain.PermissionSets.Events;
using Atlas.Domain.Sites;
using Atlas.Domain.Sites.Events;
using Atlas.Domain.Topics;
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

        public async Task EnsureDefaultSiteInitializedAsync()
        {
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Roles
            if (await roleManager.RoleExistsAsync(Consts.RoleNameAdmin) == false)
            {
                await roleManager.CreateAsync(new IdentityRole(Consts.RoleNameAdmin));
            }

            // Users
            var userAdmin = await userManager.FindByEmailAsync(_configuration["DefaultAdminUserEmail"]);

            if (userAdmin != null)
            {
                userAdmin = new IdentityUser
                {
                    Email = _configuration["DefaultAdminUserEmail"],
                    UserName = _configuration["DefaultAdminUserName"]
                };

                await userManager.CreateAsync(userAdmin, _configuration["DefaultAdminUserPassword"]);

                var code = await userManager.GenerateEmailConfirmationTokenAsync(userAdmin);
                await userManager.ConfirmEmailAsync(userAdmin, code);
            }

            await userManager.AddToRoleAsync(userAdmin, Consts.RoleNameAdmin);

            // Members
            var memberAdmin = await _dbContext.Members.FirstOrDefaultAsync(x => x.UserId == userAdmin.Id);

            if (memberAdmin == null)
            {
                memberAdmin = new Member(userAdmin.Id, "Admin");
                _dbContext.Members.Add(memberAdmin);
            }

            // Site
            var site = await _dbContext.Sites.FirstOrDefaultAsync(x => x.Name == "Default");

            if (site != null)
            {
                return;
            }

            site = new Site("Default", "Atlas");
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

            // Permission Sets
            var permissionSetDefault = new PermissionSet(site.Id, "Default", new List<PermissionCommand>
            {
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdAll},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdAll},
                new PermissionCommand{Type = PermissionType.Read, RoleId = Consts.RoleIdAll},
                new PermissionCommand{Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered}
            });
            _dbContext.PermissionSets.Add(permissionSetDefault);
            _dbContext.Events.Add(new Event(new PermissionSetCreated
            {
                SiteId = permissionSetDefault.SiteId,
                MemberId = null,
                TargetId = permissionSetDefault.Id,
                TargetType = typeof(PermissionSet).Name,
                Name = permissionSetDefault.Name
            }));

            var permissionSetMembersOnly = new PermissionSet(site.Id, "Members Only", new List<PermissionCommand>
            {
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Read, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered}
            });
            _dbContext.PermissionSets.Add(permissionSetMembersOnly);
            _dbContext.Events.Add(new Event(new PermissionSetCreated
            {
                SiteId = permissionSetMembersOnly.SiteId,
                MemberId = null,
                TargetId = permissionSetMembersOnly.Id,
                TargetType = typeof(PermissionSet).Name,
                Name = permissionSetMembersOnly.Name
            }));

            var permissionSetAdminOnly = new PermissionSet(site.Id, "Admin Only", new List<PermissionCommand>());
            _dbContext.PermissionSets.Add(permissionSetAdminOnly);
            _dbContext.Events.Add(new Event(new PermissionSetCreated
            {
                SiteId = permissionSetAdminOnly.SiteId,
                MemberId = null,
                TargetId = permissionSetAdminOnly.Id,
                TargetType = typeof(PermissionSet).Name,
                Name = permissionSetAdminOnly.Name
            }));

            // Categories
            var categoryGeneral = new Category(site.Id, "General", 1, permissionSetDefault.Id);
            _dbContext.Categories.Add(categoryGeneral);
            _dbContext.Events.Add(new Event(new CategoryCreated
            {
                SiteId = categoryGeneral.SiteId,
                MemberId = null,
                TargetId = categoryGeneral.Id,
                TargetType = typeof(Category).Name,
                Name = categoryGeneral.Name,
                PermissionSetId = categoryGeneral.PermissionSetId,
                SortOrder = categoryGeneral.SortOrder
            }));

            // Forums
            var forumWelcome = new Forum(categoryGeneral.Id, "Welcome", "Welcome Forum", 1);
            _dbContext.Forums.Add(forumWelcome);
            _dbContext.Events.Add(new Event(new ForumCreated
            {
                SiteId = categoryGeneral.SiteId,
                MemberId = null,
                TargetId = forumWelcome.Id,
                TargetType = typeof(Forum).Name,
                Name = forumWelcome.Name,
                PermissionSetId = forumWelcome.PermissionSetId,
                SortOrder = forumWelcome.SortOrder
            }));

            var forumMembersOnly = new Forum(categoryGeneral.Id, "Members Only", "Members Only Forum", 2, permissionSetMembersOnly.Id);
            _dbContext.Forums.Add(forumMembersOnly);
            _dbContext.Events.Add(new Event(new ForumCreated
            {
                SiteId = categoryGeneral.SiteId,
                MemberId = null,
                TargetId = forumMembersOnly.Id,
                TargetType = typeof(Forum).Name,
                Name = forumMembersOnly.Name,
                PermissionSetId = forumMembersOnly.PermissionSetId,
                SortOrder = forumMembersOnly.SortOrder
            }));

            var forumAdminOnly = new Forum(categoryGeneral.Id, "Admin Only", "Admin Only Forum", 3, permissionSetAdminOnly.Id);
            _dbContext.Forums.Add(forumAdminOnly);
            _dbContext.Events.Add(new Event(new ForumCreated
            {
                SiteId = categoryGeneral.SiteId,
                MemberId = null,
                TargetId = forumAdminOnly.Id,
                TargetType = typeof(Forum).Name,
                Name = forumAdminOnly.Name,
                PermissionSetId = forumAdminOnly.PermissionSetId,
                SortOrder = forumAdminOnly.SortOrder
            }));

            // Topics
            var topicWelcome = new Topic(forumWelcome.Id, memberAdmin.Id, "Welcome to Atlas!", "Welcome...", StatusType.Published);
            _dbContext.Topics.Add(topicWelcome);
            _dbContext.Events.Add(new Event(site.Id,
                topicWelcome.MemberId,
                EventType.Created,
                typeof(Topic),
                topicWelcome.Id,
                new
                {
                    topicWelcome.ForumId,
                    topicWelcome.Title,
                    topicWelcome.Content,
                    topicWelcome.Status
                }));

            // Save all changes
            await _dbContext.SaveChangesAsync();
        }
    }
}