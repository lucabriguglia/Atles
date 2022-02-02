using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Client.Services.Storage;

public interface IBrowserStorageService<T> where T : IBrowserStorageItem
{
    Task<List<T>> GetList();
    Task AddToList(T request);
    Task RemoveFromList(T request);
    Task Delete();
}