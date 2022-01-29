using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atles.Client.Services.Api
{
    public class ApiServiceAnonymous
    {
        private readonly HttpClient _httpClient;

        public ApiServiceAnonymous(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri)
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }
    }
}
