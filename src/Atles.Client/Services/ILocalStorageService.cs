namespace Atles.Client.Services;

public interface ILocalStorageService<T> : IBrowserStorageService<T> where T : IBrowserStorageCommand
{
}