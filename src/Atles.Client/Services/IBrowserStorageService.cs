using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atles.Client.Services;

public interface IBrowserStorageService<T> where T : IBrowserStorageCommand
{
    Task<List<T>> GetList();
    Task AddToList(T request);
    Task RemoveFromList(T request);
    Task Delete();
}