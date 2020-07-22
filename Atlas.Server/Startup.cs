using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Atlas.Data;
using Atlas.Data.Caching;
using Atlas.Server.Services;
using Microsoft.AspNetCore.Identity;
using Atlas.Data.Rules;
using Atlas.Data.Services;
using Atlas.Domain.ForumGroups;
using FluentValidation;
using Atlas.Domain.ForumGroups.Commands;
using Atlas.Domain.ForumGroups.Validators;
using Atlas.Domain.PermissionSets;
using Atlas.Shared;
using Atlas.Data.Builders.Admin;
using Atlas.Domain;
using Atlas.Shared.Models.Admin.ForumGroups;
using Atlas.Domain.Forums;
using Atlas.Domain.Forums.Commands;
using Atlas.Domain.Forums.Validators;
using Atlas.Shared.Forums;

namespace Atlas.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AtlasDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AtlasConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("ApplicationConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // https://github.com/dotnet/aspnetcore/issues/20436#issuecomment-607718936
            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, ApplicationDbContext>(options => 
                {
                    options.IdentityResources["openid"].UserClaims.Add("role");
                    options.ApiResources.Single().UserClaims.Add("role");
                });

            // Need to do this as it maps "role" to ClaimTypes.Role and causes issues
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("role");

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));
            });

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.Scan(s => s
                .FromAssembliesOf(typeof(Startup), typeof(Site), typeof(IndexModel), typeof(AtlasDbContext))
                .AddClasses()
                .AsImplementedInterfaces());

            //services.AddTransient<IInstallationService, InstallationService>();
            //services.AddScoped<IContextService, ContextService>();
            //services.AddTransient<ICacheManager, CacheManager>();
            //services.AddTransient<IForumGroupRules, ForumGroupRules>();
            //services.AddTransient<IPermissionSetRules, PermissionSetRules>();
            //services.AddTransient<IForumGroupService, ForumGroupService>();
            //services.AddTransient<IValidator<CreateForumGroup>, CreateForumGroupValidator>();
            //services.AddTransient<IValidator<UpdateForumGroup>, UpdateForumGroupValidator>();
            //services.AddTransient<IEventModelBuilder, EventModelBuilder>();
            //services.AddTransient<IForumGroupModelBuilder, ForumGroupModelBuilder>();

            //services.AddTransient<IForumRules, ForumRules>();
            //services.AddTransient<IForumService, ForumService>();
            //services.AddTransient<IForumModelBuilder, ForumModelBuilder>();
            //services.AddTransient<IValidator<CreateForum>, CreateForumValidator>();
            //services.AddTransient<IValidator<UpdateForum>, UpdateForumValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            AtlasDbContext atlasDbContext,
            ApplicationDbContext applicationDbContext,
            IInstallationService installationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            atlasDbContext.Database.Migrate();
            applicationDbContext.Database.Migrate();
            installationService.EnsureAdminUserInitializedAsync().Wait();
            installationService.EnsureDefaultSiteInitializedAsync().Wait();
        }
    }
}
