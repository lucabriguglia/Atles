using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Atles.Client.Services.Storage
{
    public abstract class BrowserStorageService<T> : IBrowserStorageService<T> where T : IBrowserStorageItem
    {
        protected readonly string StorageType;
        private readonly IJSRuntime _jSRuntime;

        protected BrowserStorageService(string storageType, IJSRuntime jSRuntime)
        {
            StorageType = storageType;
            _jSRuntime = jSRuntime;
        }

        public async Task<List<T>> GetList()
        {
            var result = new List<T>();

            var data = await _jSRuntime.InvokeAsync<string>($"{StorageType}.getItem", typeof(T).Name).ConfigureAwait(false);

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

            await _jSRuntime.InvokeVoidAsync($"{StorageType}.setItem", typeof(T).Name, data).ConfigureAwait(false);
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

            await _jSRuntime.InvokeVoidAsync($"{StorageType}.setItem", typeof(T).Name, data).ConfigureAwait(false);
        }

        public async Task Delete()
        {
            await _jSRuntime.InvokeVoidAsync($"{StorageType}.removeItem", typeof(T).Name).ConfigureAwait(false);
        }
    }
}
