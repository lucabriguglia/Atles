using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atles.Client.Services.Api
{
    public class ApiServiceAuthenticated
    {
        private readonly HttpClient _httpClient;

        public ApiServiceAuthenticated(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri)
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
        {
            return await _httpClient.PostAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return await _httpClient.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await _httpClient.DeleteAsync(requestUri);
        }
    }
}