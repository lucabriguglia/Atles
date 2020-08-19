using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Atlas.Client.Services;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Atlas.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("Atlas.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Atlas.ServerAPI"));

            builder.Services.AddHttpClient<AnonymousService>(client => 
            { 
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); 
            });

            builder.Services.AddHttpClient<AuthenticatedService>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            }).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped<ApiService>();

            builder.Services.AddApiAuthorization();

            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));
            });

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

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
}
