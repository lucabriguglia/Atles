using Microsoft.JSInterop;

namespace Atles.Client.Services.Storage;

public class SessionStorageService<T> : BrowserStorageService<T>, ISessionStorageService<T> where T : IBrowserStorageItem
{
    public SessionStorageService(IJSRuntime jSRuntime) : base("sessionStorage", jSRuntime)
    {
    }
}