using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Data;
using Atlas.Domain;
using Atlas.Domain.Categories;
using Atlas.Domain.Forums;
using Atlas.Domain.PermissionSets;
using Atlas.Domain.PermissionSets.Commands;
using Atlas.Domain.Posts;
using Atlas.Domain.Sites;
using Atlas.Domain.Users;
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
            if (await _dbContext.Sites.AnyAsync(x => x.Name == "Third"))
            {
                return;
            }

            //var site = new Site(new Guid("bbb7f78b-47f2-4b36-8d87-5cc3899f1c52"), "Default", "Default");
            var site = new Site(new Guid("b8b1bad1-900a-442b-86cf-9987b7e4163e"), "Third", "Third");
            _dbContext.Sites.Add(site);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(Site),
                site.Id,
                new
                {
                    site.Name,
                    site.Title,
                    site.PublicTheme,
                    site.PublicCss,
                    site.AdminTheme,
                    site.AdminCss
                }));

            var roleManager = _serviceProvider.GetRequiredService<RoleManager<SiteRole>>();
            var userManager = _serviceProvider.GetRequiredService<UserManager<SiteUser>>();

            // Roles
            if (await roleManager.RoleExistsAsync(Consts.RoleNameAdmin) == false)
            {
                await roleManager.CreateAsync(new SiteRole(site.Id, Consts.RoleNameAdmin));
            }
            var roleAdmin = await roleManager.FindByNameAsync(Consts.RoleNameAdmin);

            if (await roleManager.RoleExistsAsync(Consts.RoleNameModerator) == false)
            {
                await roleManager.CreateAsync(new SiteRole(site.Id, Consts.RoleNameModerator));
            }
            var roleModerator = await roleManager.FindByNameAsync(Consts.RoleNameModerator);

            // Identity Users
            var userAdmin = await userManager.FindByEmailAsync(_configuration["DefaultAdminUserEmail"]);
            if (userAdmin == null)
            {
                userAdmin = new SiteUser
                {
                    SiteId = site.Id,
                    Email = _configuration["DefaultAdminUserEmail"],
                    UserName = _configuration["DefaultAdminUserName"]
                };
                await userManager.CreateAsync(userAdmin, _configuration["DefaultAdminUserPassword"]);
                var code = await userManager.GenerateEmailConfirmationTokenAsync(userAdmin);
                await userManager.ConfirmEmailAsync(userAdmin, code);
            }
            await userManager.AddToRoleAsync(userAdmin, Consts.RoleNameAdmin);

            var userNormalId = string.Empty;
            if (_configuration["CreateDefaultNormalUser"] == "true")
            {
                var userNormal = await userManager.FindByEmailAsync(_configuration["DefaultNormalUserEmail"]);
                if (userNormal == null)
                {
                    userNormal = new SiteUser
                    {
                        SiteId = site.Id,
                        Email = _configuration["DefaultNormalUserEmail"],
                        UserName = _configuration["DefaultNormalUserName"]
                    };
                    await userManager.CreateAsync(userNormal, _configuration["DefaultNormalUserPassword"]);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(userNormal);
                    await userManager.ConfirmEmailAsync(userNormal, code);
                    userNormalId = userNormal.Id;
                }
            }

            var userModeratorId = string.Empty;
            if (_configuration["CreateDefaultModeratorUser"] == "true")
            {
                var userModerator = await userManager.FindByEmailAsync(_configuration["DefaultModeratorUserEmail"]);
                if (userModerator == null)
                {
                    userModerator = new SiteUser
                    {
                        SiteId = site.Id,
                        Email = _configuration["DefaultModeratorUserEmail"],
                        UserName = _configuration["DefaultModeratorUserName"]
                    };
                    await userManager.CreateAsync(userModerator, _configuration["DefaultModeratorUserPassword"]);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(userModerator);
                    await userManager.ConfirmEmailAsync(userModerator, code);
                    userModeratorId = userModerator.Id;
                }
                await userManager.AddToRoleAsync(userModerator, Consts.RoleNameModerator);
            }

            // Users
            var memberAdmin = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == userAdmin.Id);
            if (memberAdmin == null)
            {
                memberAdmin = new User(userAdmin.Id, _configuration["DefaultAdminUserEmail"], _configuration["DefaultAdminUserDisplayName"]);
                memberAdmin.Confirm();
                _dbContext.Users.Add(memberAdmin);
                _dbContext.Events.Add(new Event(site.Id,
                    null,
                    EventType.Created,
                    typeof(User),
                    memberAdmin.Id,
                    new
                    {
                        UserId = memberAdmin.IdentityUserId,
                        memberAdmin.Email,
                        memberAdmin.DisplayName
                    }));
            }

            if (_configuration["CreateDefaultNormalUser"] == "true")
            {
                var memberNormal = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == userNormalId);
                if (memberNormal == null)
                {
                    memberNormal = new User(userNormalId, _configuration["DefaultNormalUserEmail"], _configuration["DefaultNormalUserDisplayName"]);
                    memberNormal.Confirm();
                    _dbContext.Users.Add(memberNormal);
                    _dbContext.Events.Add(new Event(site.Id,
                        null,
                        EventType.Created,
                        typeof(User),
                        memberNormal.Id,
                        new
                        {
                            UserId = memberNormal.IdentityUserId,
                            memberNormal.Email,
                            memberNormal.DisplayName
                        }));
                }
            }

            if (_configuration["CreateDefaultModeratorUser"] == "true")
            {
                var memberModerator = await _dbContext.Users.FirstOrDefaultAsync(x => x.IdentityUserId == userModeratorId);
                if (memberModerator == null)
                {
                    memberModerator = new User(userModeratorId, _configuration["DefaultModeratorUserEmail"], _configuration["DefaultModeratorUserDisplayName"]);
                    memberModerator.Confirm();
                    _dbContext.Users.Add(memberModerator);
                    _dbContext.Events.Add(new Event(site.Id,
                        null,
                        EventType.Created,
                        typeof(User),
                        memberModerator.Id,
                        new
                        {
                            UserId = memberModerator.IdentityUserId,
                            memberModerator.Email,
                            memberModerator.DisplayName
                        }));
                }
            }

            // Permission Sets
            var permissionSetDefault = new PermissionSet(site.Id, "Default", new List<PermissionCommand>
            {
                new PermissionCommand{Type = PermissionType.ViewForum, RoleId = Consts.RoleIdAll},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdAll},
                new PermissionCommand{Type = PermissionType.Read, RoleId = Consts.RoleIdAll},

                new PermissionCommand{Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered},

                new PermissionCommand{Type = PermissionType.Moderate, RoleId = roleModerator.Id},

                new PermissionCommand{Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Moderate, RoleId = roleAdmin.Id}
            });
            _dbContext.PermissionSets.Add(permissionSetDefault);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(PermissionSet),
                permissionSetDefault.Id,
                new
                {
                    permissionSetDefault.Name
                }));

            var permissionSetMembersOnly = new PermissionSet(site.Id, "Members Only", new List<PermissionCommand>
            {
                new PermissionCommand{Type = PermissionType.ViewForum, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Read, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Start, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = Consts.RoleIdRegistered},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = Consts.RoleIdRegistered},

                new PermissionCommand{Type = PermissionType.Moderate, RoleId = roleModerator.Id},

                new PermissionCommand{Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Moderate, RoleId = roleAdmin.Id}
            });
            _dbContext.PermissionSets.Add(permissionSetMembersOnly);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(PermissionSet),
                permissionSetMembersOnly.Id,
                new
                {
                    permissionSetMembersOnly.Name
                }));

            var permissionSetAdminOnly = new PermissionSet(site.Id, "Admin Only", new List<PermissionCommand>
            {
                new PermissionCommand{Type = PermissionType.ViewForum, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.ViewTopics, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Read, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Start, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Reply, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Edit, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Delete, RoleId = roleAdmin.Id},
                new PermissionCommand{Type = PermissionType.Moderate, RoleId = roleAdmin.Id}
            });
            _dbContext.PermissionSets.Add(permissionSetAdminOnly);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(PermissionSet),
                permissionSetAdminOnly.Id,
                new
                {
                    permissionSetAdminOnly.Name
                }));

            // Categories
            var categoryGeneral = new Category(site.Id, "General", 1, permissionSetDefault.Id);
            _dbContext.Categories.Add(categoryGeneral);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(Category),
                categoryGeneral.Id,
                new
                {
                    categoryGeneral.Name,
                    categoryGeneral.PermissionSetId,
                    categoryGeneral.SortOrder
                }));

            // Forums
            var forumWelcome = new Forum(categoryGeneral.Id, "Welcome", "welcome", "Welcome Forum", 1);
            _dbContext.Forums.Add(forumWelcome);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(Forum),
                forumWelcome.Id,
                new
                {
                    forumWelcome.Name,
                    forumWelcome.CategoryId,
                    forumWelcome.PermissionSetId,
                    forumWelcome.SortOrder
                }));

            var forumMembersOnly = new Forum(categoryGeneral.Id, "Members Only", "members-only", "Members Only Forum", 2, permissionSetMembersOnly.Id);
            _dbContext.Forums.Add(forumMembersOnly);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(Forum),
                forumMembersOnly.Id,
                new
                {
                    forumMembersOnly.Name,
                    forumMembersOnly.CategoryId,
                    forumMembersOnly.PermissionSetId,
                    forumMembersOnly.SortOrder
                }));

            var forumAdminOnly = new Forum(categoryGeneral.Id, "Admin Only", "admin-only", "Admin Only Forum", 3, permissionSetAdminOnly.Id);
            _dbContext.Forums.Add(forumAdminOnly);
            _dbContext.Events.Add(new Event(site.Id,
                null,
                EventType.Created,
                typeof(Forum),
                forumAdminOnly.Id,
                new
                {
                    forumAdminOnly.Name,
                    forumAdminOnly.CategoryId,
                    forumAdminOnly.PermissionSetId,
                    forumAdminOnly.SortOrder
                }));

            // Topics
            var topicWelcomeTitle = "Welcome to Atlas!";
            var topicWelcome = Post.CreateTopic(forumWelcome.Id, memberAdmin.Id, topicWelcomeTitle, topicWelcomeTitle.ToSlug(), "Welcome...", StatusType.Published);
            _dbContext.Posts.Add(topicWelcome);
            _dbContext.Events.Add(new Event(site.Id,
                topicWelcome.CreatedBy,
                EventType.Created,
                typeof(Post),
                topicWelcome.Id,
                new
                {
                    topicWelcome.ForumId,
                    topicWelcome.Title,
                    topicWelcome.Content,
                    topicWelcome.Status
                }));
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