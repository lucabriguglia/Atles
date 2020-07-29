using System;
using System.Net.Http;
using System.Threading.Tasks;
using Atlas.Client.Clients;
using Atlas.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

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

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Atlas.ServerAPI"));

            builder.Services.AddHttpClient<AnonymousService>(client => 
            { 
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); 
            });

            builder.Services.AddHttpClient<AuthenticatedService>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            }).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddApiAuthorization();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));
            });

            builder.Services.AddTransient<ISecurityService, SecurityService>();

            await builder.Build().RunAsync();
        }
    }
}
