using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Atlas.Client.Services;
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

            //var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            //var result = await jsInterop.InvokeAsync<string>("blazorCulture.get");
            //if (result != null)
            //{
            //    var culture = new CultureInfo(result);
            //    CultureInfo.DefaultThreadCurrentCulture = culture;
            //    CultureInfo.DefaultThreadCurrentUICulture = culture;
            //}

            var culture = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }
    }
}
