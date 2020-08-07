using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atlas.Client.Services
{
    public class AnonymousService
    {
        private readonly HttpClient _httpClient;

        public AnonymousService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }
    }
}
