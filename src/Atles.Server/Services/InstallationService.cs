using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atles.Commands.Handlers;
using Atles.Commands.PermissionSets;
using Atles.Core.Extensions;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Categories;
using Atles.Events.Forums;
using Atles.Events.PermissionSets;
using Atles.Events.Posts;
using Atles.Events.Sites;
using Atles.Events.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atles.Server.Services
{
    public class InstallationService : IInstallationService
    {
        private readonly AtlesDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public InstallationService(AtlesDbContext dbContext, 
            IConfiguration configuration, 
            IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public async Task EnsureDefaultSiteInitializedAsync()
        {
            if (await _dbContext.Sites.AnyAsync(x => x.Name == "Default"))
            {
                return;
            }

            var site = new Site("Default", "Atles");
            _dbContext.Sites.Add(site);
            var siteCreated = new SiteCreated
            {
                Name = site.Name,
                Title = site.Title,
                PublicTheme = site.PublicTheme,
                PublicCss = site.PublicCss,
                TargetId = site.Id,
                TargetType = nameof(Site),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(siteCreated.ToDbEntity());
            var roleManager = _serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Roles
            if (await roleManager.RoleExistsAsync(Consts.RoleNameAdmin) == false)
            {
                await roleManager.CreateAsync(new IdentityRole(Consts.RoleNameAdmin));
            }
            var roleAdmin = await roleManager.FindByNameAsync(Consts.RoleNameAdmin);

            if (await roleManager.RoleExistsAsync(Consts.RoleNameModerator) == false)
            {
                await roleManager.CreateAsync(new IdentityRole(Consts.RoleNameModerator));
            }
            var roleModerator = await roleManager.FindByNameAsync(Consts.RoleNameModerator);

            // Identity Users
            var userAdmin = await userManager.FindByEmailAsync(_configuration["DefaultAdminUserEmail"]);
            if (userAdmin == null)
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

            // Users
            var memberAdmin = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == userAdmin.Id);
            if (memberAdmin == null)
            {
                memberAdmin = new User(userAdmin.Id, _configuration["DefaultAdminUserEmail"], _configuration["DefaultAdminUserDisplayName"]);
                memberAdmin.Confirm();
                _dbContext.Users.Add(memberAdmin);
                var userCreated = new UserCreated
                {
                    IdentityUserId = memberAdmin.IdentityUserId,
                    Email = memberAdmin.Email,
                    DisplayName = memberAdmin.DisplayName,
                    TargetId = memberAdmin.Id,
                    TargetType = nameof(User),
                    SiteId = site.Id,
                    UserId = null
                };
                _dbContext.Events.Add(userCreated.ToDbEntity());
            }

            // Permission Sets
            var permissionsDefault = new List<PermissionCommand>
            {
                new PermissionCommand {Type = PermissionType.ViewForum, RoleId = Consts.RoleIdAll},
                new PermissionCommand {Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdAll},
                new PermissionCommand {Type = PermissionType.Read, RoleId = Consts.RoleIdAll},

                new PermissionCommand {Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Reactions, RoleId = Consts.RoleIdRegistered},

                new PermissionCommand {Type = PermissionType.Moderate, RoleId = roleModerator.Id},

                new PermissionCommand {Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Moderate, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reactions, RoleId = roleAdmin.Id}
            };
            var permissionSetDefault = new PermissionSet(site.Id, "Default", permissionsDefault.ToDomainPermissions());
            _dbContext.PermissionSets.Add(permissionSetDefault);
            var permissionSetDefaultCreated = new PermissionSetCreated
            {
                Name = permissionSetDefault.Name,
                Permissions = permissionsDefault.ToDomainPermissions(),
                TargetId = permissionSetDefault.Id,
                TargetType = nameof(PermissionSet),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(permissionSetDefaultCreated.ToDbEntity());

            var permissionsMembersOnly = new List<PermissionCommand>
            {
                new PermissionCommand {Type = PermissionType.ViewForum, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Read, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand {Type = PermissionType.Reactions, RoleId = Consts.RoleIdRegistered},

                new PermissionCommand {Type = PermissionType.Moderate, RoleId = roleModerator.Id},

                new PermissionCommand {Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Moderate, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reactions, RoleId = roleAdmin.Id}
            };
            var permissionSetMembersOnly = new PermissionSet(site.Id, "Members Only", permissionsMembersOnly.ToDomainPermissions());
            _dbContext.PermissionSets.Add(permissionSetMembersOnly);
            var permissionSetMembersOnlyCreated = new PermissionSetCreated
            {
                Name = permissionSetMembersOnly.Name,
                Permissions = permissionsMembersOnly.ToDomainPermissions(),
                TargetId = permissionSetMembersOnly.Id,
                TargetType = nameof(PermissionSet),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(permissionSetMembersOnlyCreated.ToDbEntity());

            var permissionsAdminOnly = new List<PermissionCommand>
            {
                new PermissionCommand {Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Moderate, RoleId = roleAdmin.Id},
                new PermissionCommand {Type = PermissionType.Reactions, RoleId = roleAdmin.Id}
            };
            var permissionSetAdminOnly = new PermissionSet(site.Id, "Admin Only", permissionsAdminOnly.ToDomainPermissions());
            _dbContext.PermissionSets.Add(permissionSetAdminOnly);
            var permissionSetAdminOnlyCreated = new PermissionSetCreated
            {
                Name = permissionSetAdminOnly.Name,
                Permissions = permissionsAdminOnly.ToDomainPermissions(),
                TargetId = permissionSetAdminOnly.Id,
                TargetType = nameof(PermissionSet),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(permissionSetAdminOnlyCreated.ToDbEntity());

            // Categories
            var categoryGeneral = new Category(site.Id, "General", 1, permissionSetDefault.Id);
            _dbContext.Categories.Add(categoryGeneral);
            var categoryGeneralCreated = new CategoryCreated
            {
                Name = categoryGeneral.Name,
                PermissionSetId = categoryGeneral.PermissionSetId,
                SortOrder = categoryGeneral.SortOrder,
                TargetId = categoryGeneral.Id,
                TargetType = nameof(Category),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(categoryGeneralCreated.ToDbEntity());

            // Forums
            var forumWelcome = new Forum(categoryGeneral.Id, "Welcome", "welcome", "Welcome Forum", 1);
            _dbContext.Forums.Add(forumWelcome);
            var forumWelcomeCreated = new ForumCreated
            {
                Name = forumWelcome.Name,
                Slug = forumWelcome.Slug,
                Description = forumWelcome.Description,
                CategoryId = forumWelcome.CategoryId,
                PermissionSetId = forumWelcome.PermissionSetId,
                SortOrder = forumWelcome.SortOrder,
                TargetId = forumWelcome.Id,
                TargetType = nameof(Forum),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(forumWelcomeCreated.ToDbEntity());

            var forumMembersOnly = new Forum(categoryGeneral.Id, "Members Only", "members-only", "Members Only Forum", 2, permissionSetMembersOnly.Id);
            _dbContext.Forums.Add(forumMembersOnly);
            var forumMembersOnlyCreated = new ForumCreated
            {
                Name = forumMembersOnly.Name,
                Slug = forumMembersOnly.Slug,
                Description = forumMembersOnly.Description,
                CategoryId = forumMembersOnly.CategoryId,
                PermissionSetId = forumMembersOnly.PermissionSetId,
                SortOrder = forumMembersOnly.SortOrder,
                TargetId = forumMembersOnly.Id,
                TargetType = nameof(Forum),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(forumMembersOnlyCreated.ToDbEntity());

            var forumAdminOnly = new Forum(categoryGeneral.Id, "Admin Only", "admin-only", "Admin Only Forum", 3, permissionSetAdminOnly.Id);
            _dbContext.Forums.Add(forumAdminOnly);
            var forumAdminOnlyCreated = new ForumCreated
            {
                Name = forumAdminOnly.Name,
                Slug = forumAdminOnly.Slug,
                Description = forumAdminOnly.Description,
                CategoryId = forumAdminOnly.CategoryId,
                PermissionSetId = forumAdminOnly.PermissionSetId,
                SortOrder = forumAdminOnly.SortOrder,
                TargetId = forumAdminOnly.Id,
                TargetType = nameof(Forum),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(forumAdminOnlyCreated.ToDbEntity());

            // Topics
            const string topicWelcomeTitle = "Welcome to Atles!";
            var topicWelcome = Post.CreateTopic(forumWelcome.Id, memberAdmin.Id, topicWelcomeTitle, topicWelcomeTitle.ToSlug(), "Welcome...", PostStatusType.Published);
            _dbContext.Posts.Add(topicWelcome);
            var topicWelcomeCreated = new TopicCreated
            {
                ForumId = topicWelcome.ForumId,
                Title = topicWelcome.Title,
                Slug = topicWelcome.Slug,
                Content = topicWelcome.Content,
                Status = topicWelcome.Status,
                TargetId = topicWelcome.Id,
                TargetType = nameof(Post),
                SiteId = site.Id,
                UserId = null
            };
            _dbContext.Events.Add(topicWelcomeCreated.ToDbEntity());
            categoryGeneral.IncreaseTopicsCount();
            forumWelcome.IncreaseTopicsCount();
            memberAdmin.IncreaseTopicsCount();

            // Save all changes
            await _dbContext.SaveChangesAsync();

            // Update last post
            var forumWelcomeToUpdate = await _dbContext.Forums.FirstOrDefaultAsync(x => x.Id == forumWelcome.Id);
            forumWelcomeToUpdate.UpdateLastPost(topicWelcome.Id);
            await _dbContext.SaveChangesAsync();
        }

        public Task InstallThemeAsync()
        {
            throw new NotImplementedException();
        }
    }
}