namespace Atles.Client.Services.Storage;

public interface ILocalStorageService<T> : IBrowserStorageService<T> where T : IBrowserStorageItem
{
}