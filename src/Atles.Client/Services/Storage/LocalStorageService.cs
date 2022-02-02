using Microsoft.JSInterop;

namespace Atles.Client.Services.Storage;

public class LocalStorageService<T> : BrowserStorageService<T>, ILocalStorageService<T> where T : IBrowserStorageItem
{
    public LocalStorageService(IJSRuntime jSRuntime) : base("localStorage", jSRuntime)
    {
    }
}