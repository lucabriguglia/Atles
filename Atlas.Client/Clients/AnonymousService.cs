using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Clients
{
    public class AnonymousService : ServiceBase
    {
        private readonly AuthenticationStateProvider _authProvider;

        public AnonymousService(HttpClient httpClient, AuthenticationStateProvider authProvider) 
            : base(httpClient)
        {
            _authProvider = authProvider;
        }

        public override async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            var userId = string.Empty;

            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                userId = authState.User.Identities.First().Claims
                    .FirstOrDefault(x => x.Type == "sub")?.Value;
            }

            HttpClient.DefaultRequestHeaders.Add("X-UserId", userId);

            return await base.GetFromJsonAsync<T>(requestUri);
        }
    }
}
