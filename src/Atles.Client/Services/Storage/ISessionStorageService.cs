namespace Atles.Client.Services.Storage;

public interface ISessionStorageService<T> : IBrowserStorageService<T> where T : IBrowserStorageItem
{
}