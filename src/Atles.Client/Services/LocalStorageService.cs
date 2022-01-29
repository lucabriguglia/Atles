using Microsoft.JSInterop;

namespace Atles.Client.Services;

public class LocalStorageService<T> : BrowserStorageService<T>, ILocalStorageService<T> where T : IBrowserStorageCommand
{
    public LocalStorageService(IJSRuntime jSRuntime) : base("localStorage", jSRuntime)
    {
    }
}