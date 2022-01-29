using Microsoft.JSInterop;

namespace Atles.Client.Services;

public class SessionStorageService<T> : BrowserStorageService<T>, ISessionStorageService<T> where T : IBrowserStorageCommand
{
    public SessionStorageService(IJSRuntime jSRuntime) : base("sessionStorage", jSRuntime)
    {
    }
}