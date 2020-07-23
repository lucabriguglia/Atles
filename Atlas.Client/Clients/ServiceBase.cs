using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atlas.Client.Clients
{

    public abstract class ServiceBase
    {
        protected readonly HttpClient _httpClient;

        public ServiceBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }

        public async Task PostAsJsonAsync(string requestUri, object data)
        {
            await _httpClient.PostAsJsonAsync(requestUri, data);
        }
    }
}
