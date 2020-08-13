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
using Atlas.Server.Services;
using Microsoft.AspNetCore.Identity;
using Atlas.Domain.Sites;
using Atlas.Models.Admin.Categories;

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
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

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
                .FromAssembliesOf(typeof(Startup), typeof(Site), typeof(IndexPageModel), typeof(AtlasDbContext))
                .AddClasses()
                .AsImplementedInterfaces());
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
            installationService.EnsureDefaultSiteInitializedAsync().Wait();
        }
    }
}
