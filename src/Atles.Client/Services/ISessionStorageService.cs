namespace Atles.Client.Services;

public interface ISessionStorageService<T> : IBrowserStorageService<T> where T : IBrowserStorageCommand
{
}