using System.IdentityModel.Tokens.Jwt;
using Atles.Commands.Handlers.Categories;
using Atles.Core;
using Atles.Core.Results;
using Atles.Core.Settings;
using Atles.Data;
using Atles.Domain;
using Atles.Events.Handlers.ReplyCreatedHandlers;
using Atles.Models.Admin.Categories;
using Atles.Queries.Handlers.Admin;
using Atles.Server.Extensions;
using Atles.Server.Middleware;
using Atles.Server.Services;
using Atles.Validators;
using Docs;
using Docs.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atles.Server;

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
        services.Configure<DatabaseSettings>(options => {
            options.AtlesConnectionString = Configuration.GetConnectionString("AtlesConnection");
            options.IdentityConnectionString = Configuration.GetConnectionString("IdentityConnection");
        });

        services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

        services.AddDbContext<AtlesDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("AtlesConnection")));

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("IdentityConnection")));

        services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityDbContext>();

        // https://github.com/dotnet/aspnetcore/issues/20436#issuecomment-607718936
        services.AddIdentityServer()
            .AddApiAuthorization<IdentityUser, IdentityDbContext>(options => 
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

        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = int.MaxValue;
            x.MultipartBodyLengthLimit = int.MaxValue;
        });

        //services.Configure<CookiePolicyOptions>(options =>
        //{
        //    options.CheckConsentNeeded = context => true;
        //    options.MinimumSameSitePolicy = SameSiteMode.None;
        //});

        services.AddDocs();
        services.AddAutoMapper();

        services.Scan(s => s
            .FromAssembliesOf(
                typeof(Startup), 
                typeof(Dispatcher), 
                typeof(CreateCategoryHandler), 
                typeof(CategoryValidator),
                typeof(CategoriesPageModel), 
                typeof(GetCategoriesIndexHandler), 
                typeof(AtlesDbContext),
                typeof(SubscriptionHandler))
            .AddClasses(classes => classes.NotInNamespaceOf(typeof(CommandResult)))
            .AsImplementedInterfaces());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app,
        IWebHostEnvironment env,
        AtlesDbContext atlasDbContext,
        IdentityDbContext identityDbContext,
        IInstallationService installationService,
        IDocumentationService documentationService)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        if (env.IsDevelopment())
        {
            //app.UseExceptionHandler("/Error");

            //app.UseDeveloperExceptionPage();
            //app.UseDatabaseErrorPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            //app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        //app.UseStaticFiles(new StaticFileOptions
        //{
        //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
        //    RequestPath = new PathString("/Uploads")
        //});

        //app.UseCookiePolicy();

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

        if (Configuration["MigrateDatabases"] == "true")
        {
            atlasDbContext.Database.Migrate();
            identityDbContext.Database.Migrate();
        }

        if (Configuration["EnsureDefaultSiteInitialized"] == "true")
        {
            installationService.EnsureDefaultSiteInitializedAsync().Wait();
        }            
            
        if (Configuration["GenerateDocumentationOnStartup"] == "true")
        {
            documentationService.Generate(typeof(Site).Assembly);
        }
    }
}
