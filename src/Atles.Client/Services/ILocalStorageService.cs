using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Client.Services;

public interface ILocalStorageService<T> where T : ILocalStorageCommand
{
    Task<List<T>> GetList();
    Task AddToList(T request);
    Task RemoveFromList(T request);
    Task DeleteList();
}