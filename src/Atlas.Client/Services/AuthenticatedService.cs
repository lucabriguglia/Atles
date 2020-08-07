﻿using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Atlas.Client.Services
{
    public class AuthenticatedService
    {
        private readonly HttpClient _httpClient;

        public AuthenticatedService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetFromJsonAsync<T>(string requestUri) where T : class
        {
            return await _httpClient.GetFromJsonAsync<T>(requestUri);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUri, object data)
        {
            return await _httpClient.PostAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await _httpClient.DeleteAsync(requestUri);
        }
    }
}