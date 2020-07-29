using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atlas.Client.Clients
{

    public abstract class ServiceBase
    {
        protected readonly HttpClient HttpClient;

        public ServiceBase(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            return await HttpClient.GetFromJsonAsync<T>(requestUri);
        }

        public async Task PostAsJsonAsync(string requestUri, object data)
        {
            await HttpClient.PostAsJsonAsync(requestUri, data);
        }
    }
}
