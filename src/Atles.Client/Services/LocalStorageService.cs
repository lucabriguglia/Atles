using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Atles.Client.Services
{
    public class LocalStorageService<T> : ILocalStorageService<T> where T : ILocalStorageCommand

    {
        private readonly IJSRuntime _jSRuntime;

        public LocalStorageService(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        public async Task<List<T>> GetList()
        {
            var result = new List<T>();

            var data = await _jSRuntime.InvokeAsync<string>("localStorage.getItem", typeof(T).Name).ConfigureAwait(false);

            if (data != null)
            {
                result = JsonSerializer.Deserialize<List<T>>(data);
            }

            return result;
        }

        public async Task AddToList(T request)
        {
            var list = await GetList();

            list.Add(request);

            var data = JsonSerializer.Serialize(list);

            await _jSRuntime.InvokeVoidAsync("localStorage.setItem", typeof(T).Name, data).ConfigureAwait(false);
        }

        public async Task RemoveFromList(T request)
        {
            var list = await GetList();

            var item = list.FirstOrDefault(x => x.Key == request.Key);

            if (item != null)
            {
                list.Remove(item);
            }

            var data = JsonSerializer.Serialize(list);

            await _jSRuntime.InvokeVoidAsync("localStorage.setItem", typeof(T).Name, data).ConfigureAwait(false);
        }

        public async Task DeleteList()
        {
            await _jSRuntime.InvokeVoidAsync("localStorage.removeItem", typeof(T).Name).ConfigureAwait(false);
        }
    }
}
