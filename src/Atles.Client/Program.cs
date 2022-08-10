using System.Globalization;
using Atles.Client.Services.Api;
using Atles.Client.Services.PostReactions;
using Atles.Client.Services.Storage;
using Atles.Client.ValidationRules;
using Atles.Models.Admin.Categories;
using Atles.Models.Admin.Forums;
using Atles.Models.Admin.PermissionSets;
using Atles.Models.Admin.Users;
using Atles.Models.Public;
using Atles.Validators;
using Atles.Validators.Users;
using Atles.Validators.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tewr.Blazor.FileReader;
using SettingsPageModel = Atles.Models.Admin.Sites.SettingsPageModel;

namespace Atles.Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddHttpClient("Atles.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Atles.ServerAPI"));

        builder.Services.AddHttpClient<ApiServiceAnonymous>(client => 
        { 
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); 
        });

        builder.Services.AddHttpClient<ApiServiceAuthenticated>(client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        }).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        builder.Services.AddScoped<ApiService>();

        builder.Services.AddApiAuthorization();

        builder.Services.AddOptions();

        builder.Services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        });

        builder.Services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });

        builder.Services.AddScoped(typeof(ILocalStorageService<>), typeof(LocalStorageService<>));
        builder.Services.AddScoped(typeof(ISessionStorageService<>), typeof(SessionStorageService<>));
        builder.Services.AddScoped(typeof(IPostReactionService), typeof(PostReactionService));

        builder.Services.AddTransient<ICategoryValidationRules, ApiCategoryValidationRules>();
        builder.Services.AddTransient<IForumValidationRules, ApiForumValidationRules>();
        builder.Services.AddTransient<IPermissionSetValidationRules, ApiPermissionSetValidationRules>();
        builder.Services.AddTransient<ITopicValidationRules, ApiTopicValidationRules>();
        builder.Services.AddTransient<IUserValidationRules, ApiUserValidationRules>();

        builder.Services.AddTransient<IValidator<CategoryFormModel.CategoryModel>, CategoryValidator>();
        builder.Services.AddTransient<IValidator<ForumFormModel.ForumModel>, ForumValidator>();
        builder.Services.AddTransient<IValidator<PermissionSetFormModel.PermissionSetModel>, PermissionSetValidator>();
        builder.Services.AddTransient<IValidator<SettingsPageModel.SiteModel>, SiteValidator>();
        builder.Services.AddTransient<IValidator<CreateUserPageModel.UserModel>, CreateUserValidator>();
        builder.Services.AddTransient<IValidator<EditUserPageModel.UserModel>, UpdateUserValidator>();

        builder.Services.AddFileReaderService(o => o.UseWasmSharedBuffer = true);

        var host = builder.Build();

        var apiService = host.Services.GetRequiredService<ApiService>();
        var site = await apiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
        var cultureName = site.Language ?? "en";
        var culture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        await host.RunAsync();
    }
}
