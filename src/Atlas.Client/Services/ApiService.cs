using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Atlas.Client.Services
{
    public class ApiService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly AnonymousService _anonymousService;
        private readonly AuthenticatedService _authenticatedService;

        public ApiService(AuthenticationStateProvider authenticationStateProvider, 
            AnonymousService anonymousService, 
            AuthenticatedService authenticatedService)
        {
            _anonymousService = anonymousService;
            _authenticatedService = authenticatedService;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            if (user.Identity.IsAuthenticated)
            {
                return await _authenticatedService.GetFromJsonAsync<T>(requestUri);
            }

            return await _anonymousService.GetFromJsonAsync<T>(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, object data)
        {
            return await _authenticatedService.PostAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await _authenticatedService.DeleteAsync(requestUri);
        }
    }
}